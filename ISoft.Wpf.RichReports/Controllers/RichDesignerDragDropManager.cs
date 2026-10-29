using DevExpress.Xpf.Accordion;
using ISoft.RichReportDesigner.Core.Fields.Display;
using ISoft.RichReportDesigner.UI.Controls;
using ISoft.Utils.Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ISoft.RichReportDesigner.UI.Controllers
{
    internal sealed class RichDesignerDragDropManager
    {
        private const string FieldDropFormat = "RichFieldDropFormate"; // keep the string value!

        private readonly RichReportDesignerControl _designer;

        private RichReportDesignerFieldsPanel _fieldsPanel;
        private XpfRichEditorControl _editor;

        private Point _dragStartPoint;
        private IRichDisplayField _draggedField;

        public RichDesignerDragDropManager(RichReportDesignerControl designerControl)
        {
            _designer = designerControl ?? throw new ArgumentNullException(nameof(designerControl));
        }

        public void Install(RichReportDesignerFieldsPanel fieldsPanel, XpfRichEditorControl editorControl)
        {
            Uninstall();

            _fieldsPanel = fieldsPanel;
            _editor = editorControl;

            Subscribe();
        }

        public void Uninstall()
        {
            Unsubscribe();

            _fieldsPanel = null;
            _editor = null;
            _draggedField = null;
        }

        private void Subscribe()
        {
            if (_fieldsPanel != null)
            {
                _fieldsPanel.PreviewMouseLeftButtonDown += OnFieldsPanelMouseLeftButtonDown;
                _fieldsPanel.PreviewMouseMove += OnFieldsPanelMouseMove;
                _fieldsPanel.PreviewMouseDoubleClick += OnFieldsPanelMouseDoubleClick;
            }

            if (_editor != null)
            {
                _editor.DragEnter += OnEditorDragEnter;
                _editor.DragOver += OnEditorDragOver;
                _editor.Drop += OnEditorDrop;
            }
        }

        private void Unsubscribe()
        {
            if (_fieldsPanel != null)
            {
                _fieldsPanel.PreviewMouseLeftButtonDown -= OnFieldsPanelMouseLeftButtonDown;
                _fieldsPanel.PreviewMouseMove -= OnFieldsPanelMouseMove;
                _fieldsPanel.PreviewMouseDoubleClick -= OnFieldsPanelMouseDoubleClick;
            }

            if (_editor != null)
            {
                _editor.DragEnter -= OnEditorDragEnter;
                _editor.DragOver -= OnEditorDragOver;
                _editor.Drop -= OnEditorDrop;
            }
        }

        // ---------- Source: fields panel ----------

        private void OnFieldsPanelMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragStartPoint = e.GetPosition(null);
            _draggedField = FindAccordionItem(e.OriginalSource as DependencyObject)?.DataContext as IRichDisplayField;
        }

        private void OnFieldsPanelMouseMove(object sender, MouseEventArgs e)
        {
            if (_draggedField == null || e.LeftButton != MouseButtonState.Pressed)
                return;

            if (!DragDropHelper.ShouldStartDrag(_dragStartPoint, e.GetPosition(null)))
                return;

            var field = _draggedField;
            _draggedField = null; // consume, so a subsequent move won't restart a drag

            DragDrop.DoDragDrop(
                _fieldsPanel,
                new DataObject(FieldDropFormat, field),
                DragDropEffects.Copy);
        }

        private void OnFieldsPanelMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FindAccordionItem(e.OriginalSource as DependencyObject)?.DataContext is not IRichDisplayField field)
                return; // header / scrollbar / empty area → let the accordion handle it

            _draggedField = null; // prevent an accidental drag after a double-click
            e.Handled = true;
            InsertDocumentField(field);
        }

        // ---------- Target: editor ----------

        private void OnEditorDragEnter(object sender, DragEventArgs e) => _editor.Focus();

        private void OnEditorDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;

            if (!TryGetField(e.Data, out _))
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            var point = e.GetPosition(_editor);
            var caret = _editor.GetDropPosition(new System.Drawing.Point((int)point.X, (int)point.Y));
            if (caret == null)
            {
                e.Effects = DragDropEffects.None;
                return;
            }

            _editor.Document.CaretPosition = caret;
            _editor.ScrollToCaret();
            e.Effects = DragDropEffects.Copy;
        }

        private void OnEditorDrop(object sender, DragEventArgs e)
        {
            e.Handled = true;

            if (TryGetField(e.Data, out var field))
                InsertDocumentField(field);
        }

        private void InsertDocumentField(IRichDisplayField field)
        {
            _editor.Focus();
            _designer.InsertField(field);
        }

        private static bool TryGetField(IDataObject data, out IRichDisplayField field)
        {
            field = data != null && data.GetDataPresent(FieldDropFormat)
                ? data.GetData(FieldDropFormat) as IRichDisplayField
                : null;

            return field != null;
        }

        private static AccordionItem FindAccordionItem(DependencyObject source)
        {
            for (var node = source; node != null; node = VisualTreeHelper.GetParent(node))
            {
                if (node is ScrollViewer) return null; // inside the content area, not the item
                if (node is AccordionItem item) return item;
            }
            return null;
        }
    }
}
