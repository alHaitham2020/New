using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Layout;
using ISoft.RichReportDesigner.Core.RichEdit.Services;
using ISoft.RichReportDesigner.UI.Controls;
using ISoft.Utils.Extentions;
using ISoft.Utils.Helpers;
using System;
using System.Windows;

namespace ISoft.RichReportDesigner.UI.Controllers
{
    //internal class RichDesignerEditorController
    //{
    //    private readonly OperationLocker _loadTemplateLocker = new();

    //    private readonly RichReportDesignerControl _designer;
    //    private readonly RichDesignerDragDropManager _dragDropManager;
    //    private XpfRichEditorControl _editor;

    //    public RichDesignerFieldController Fields { get; }

    //    public RichDesignerTemplateController Template { get; }

    //    public RichDesignerCommandsHandler CommandsHandler { get; }

    //    public RichDesignerEditorController(RichReportDesignerControl designer)
    //    {
    //        ArgumentGuardHelper.ArgumentNotNull(designer, nameof(designer));

    //        _designer = designer;
    //        _dragDropManager = new RichDesignerDragDropManager(designer);

    //        Fields = new RichDesignerFieldController(designer);
    //        Template = new RichDesignerTemplateController(designer);
    //        CommandsHandler = new RichDesignerCommandsHandler(designer, Template);
    //    }

    //    public void Install(RichReportDesignerFieldsPanel fieldsPanel, XpfRichEditorControl editorControl)
    //    {
    //        _editor = editorControl;
    //        _dragDropManager.Install(fieldsPanel, editorControl);
    //    }

    //    public void UnInstall()
    //    {

    //    }

    //    private void AttachToEditor()
    //    {
    //        _editor.DoIfNotNull(editor =>
    //        {
    //            // Set up event handlers
    //            editor.Loaded += Editor_Loaded;
    //            editor.CalculateDocumentVariable += Editor_CalculateDocumentVariable;
    //            editor.InitializeDocument += Editor_InitializeDocument;
    //            editor.DocumentLoaded += Editor_DocumentLoaded;
    //            editor.BeforePagePaint += Editor_BeforePagePaint;
    //        });
    //    }

    //    private void DetachFromEditor()
    //    {
    //        _editor.DoIfNotNull(editor =>
    //        {
    //            // Set up event handlers
    //            editor.Loaded -= Editor_Loaded;
    //            editor.CalculateDocumentVariable -= Editor_CalculateDocumentVariable;
    //            editor.InitializeDocument -= Editor_InitializeDocument;
    //            editor.DocumentLoaded -= Editor_DocumentLoaded;
    //            editor.BeforePagePaint -= Editor_BeforePagePaint;
    //        });
    //    }

    //    protected void SetEditorFocus()
    //    {
    //        if (_editor == null)
    //            return;

    //        _editor.FocusKeyCodeTextBox();
    //    }

    //    #region Editor Events Handelrs

    //    private void Editor_Loaded(object sender, RoutedEventArgs e)
    //    {
    //        SetEditorFocus();
    //    }

    //    private void Editor_DocumentLoaded(object sender, EventArgs e)
    //    {
    //        Fields.HighlightsFields();
    //    }

    //    private void Editor_InitializeDocument(object sender, EventArgs e)
    //    {
    //        _editor.ConfigureDocumentDefaultProperties(_designer.FlowDirection.IsRightToLeft());
    //    }

    //    private void Editor_CalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e)
    //    {
    //        _designer.GetService<IRichDocumentVariableManagerService>().DoIfNotNull(s =>
    //        {
    //            s.Calculate(e);
    //        });
    //    }

    //    // can called by _editor.Refresh()
    //    private void Editor_BeforePagePaint(object sender, BeforePagePaintEventArgs e)
    //    {
    //        // Only apply custom drawing to the screen, not to the printer
    //        if (e.CanvasOwnerType != CanvasOwnerType.Control || !_designer.IsReady)
    //            return;

    //        // Assign the custom painter
    //        e.Painter = new PagePainter();
    //    }

    //    #endregion

    //}


    internal class RichDesignerEditorController
    {
        private readonly RichReportDesignerControl _designer;
        private XpfRichEditorControl _editor;

        public RichDesignerEditorController(RichReportDesignerControl designer)
        {
            ArgumentGuardHelper.ArgumentNotNull(designer, nameof(designer));
            _designer = designer;
        }

        public void Install(XpfRichEditorControl editorControl)
        {
            DetachFromEditor();
            _editor = editorControl;
            AttachToEditor();
        }

        public void UnInstall()
        {
            DetachFromEditor();
        }

        private void AttachToEditor()
        {
            _editor.DoIfNotNull(editor =>
            {
                // Set up event handlers
                editor.Loaded += Editor_Loaded;
                editor.CalculateDocumentVariable += Editor_CalculateDocumentVariable;
                editor.InitializeDocument += Editor_InitializeDocument;
                editor.DocumentLoaded += Editor_DocumentLoaded;
                editor.BeforePagePaint += Editor_BeforePagePaint;
            });
        }

        private void DetachFromEditor()
        {
            _editor.DoIfNotNull(editor =>
            {
                // Set up event handlers
                editor.Loaded -= Editor_Loaded;
                editor.CalculateDocumentVariable -= Editor_CalculateDocumentVariable;
                editor.InitializeDocument -= Editor_InitializeDocument;
                editor.DocumentLoaded -= Editor_DocumentLoaded;
                editor.BeforePagePaint -= Editor_BeforePagePaint;
            });
        }

        protected void SetEditorFocus()
        {
            if (_editor == null)
                return;

            _editor.FocusKeyCodeTextBox();
        }

        #region Editor Events Handelrs

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            SetEditorFocus();
        }

        private void Editor_DocumentLoaded(object sender, EventArgs e)
        {
            _designer.HighlightsFields();
        }

        private void Editor_InitializeDocument(object sender, EventArgs e)
        {
            _editor.ConfigureDocumentDefaultProperties(_designer.FlowDirection.IsRightToLeft());
        }

        private void Editor_CalculateDocumentVariable(object sender, CalculateDocumentVariableEventArgs e)
        {
            _designer.GetService<IRichDocumentVariableManagerService>().DoIfNotNull(s =>
            {
                s.Calculate(e);
            });
        }

        // can called by _editor.Refresh()
        private void Editor_BeforePagePaint(object sender, BeforePagePaintEventArgs e)
        {
            // Only apply custom drawing to the screen, not to the printer
            if (e.CanvasOwnerType != CanvasOwnerType.Control || !_designer.IsReady)
                return;

            // Assign the custom painter
            e.Painter = new PagePainter();
        }

        #endregion
    }
}
