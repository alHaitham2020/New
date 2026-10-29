using ISoft.Wpf.RichReports.Editor.Commands;
using ISoft.Wpf.RichReports.ViewModels;
using ISoft.Wpf.RichReports.Views;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ISoft.Wpf.RichReports.Controls
{
    /// <summary>
    /// represent a internal hosted rich editor thats inhert from devexpress wpf richeditcontrol
    /// </summary>
    public class XpfRichEditorControl : RichEditControl, IRichEditorControl
    {
        public static readonly DependencyProperty XpfCommandProviderProperty;
        public static readonly DependencyProperty FieldHighlightingEnabledProperty;

        static XpfRichEditorControl() 
        {
            XpfRichEditLocalizer.Active = new XpfRichEditControlLocalizer();
            XtraRichEditLocalizer.Active = new XtraRichEditControlLocalizer();

            //RichEditControlCompatibility.

            DependencyPropertyRegistrator<XpfRichEditorControl>
             .New()
             .Register(nameof(FieldHighlightingEnabled), out FieldHighlightingEnabledProperty, true, d => d.OnFieldHighlightingEnabledChanged())
             .Register<XpfRichCommandProvider>(nameof(XpfCommandProvider), out XpfCommandProviderProperty, null);
        }

        public XpfRichEditorControl() : base()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            fieldKeyboardGuard = new RichEditFieldKeyboardGuard(this);
            XpfCommandProvider = new XpfRichCommandProvider(this);
        }

        #region Proprties

        private RichEditFieldKeyboardGuard fieldKeyboardGuard;

        public XpfRichCommandProvider XpfCommandProvider
        {
            get => (XpfRichCommandProvider)GetValue(XpfCommandProviderProperty);
            set => SetValue(XpfCommandProviderProperty, value);
        }

        public bool FieldHighlightingEnabled
        {
            get => (bool)GetValue(FieldHighlightingEnabledProperty);
            set => SetValue(FieldHighlightingEnabledProperty, value);
        }

        #endregion

        #region Ovverides
        public new static XpfRichEditorControl GetRichEdit(DependencyObject element)
        {
            return (XpfRichEditorControl)element.GetValue(RichEditProperty);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            ConfigureDefaultBehavior();
        }

        protected override void AddServicesPlatformSpecific()
        {
            base.AddServicesPlatformSpecific();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                ReplaceService<IRichEditCommandFactoryService>(new RichEditorCommandsFactoryService(this));
                ReplaceService<IRichEditPrintingService>(new RichDocumentEditPrintingService());
                AddService(typeof(IRichEditExpressionContextFactoryService), new RichEditExpressionContextFactoryService(this));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                XpfCommandProvider.Dispose();
                if(fieldKeyboardGuard != null)
                {
                    fieldKeyboardGuard.UnInstall();
                    fieldKeyboardGuard = null;
                }
            }

            base.Dispose(disposing);
        }

        protected override void ShowDXDialog(
            UserControl form,
            FrameworkElement rootElement,
            DialogClosedDelegate onDialogClosed,
            string title,
            ResizeMode resizeMode,
            Func<bool> onOkClickDelegate = null)
        {
            if (form != null)
            {
                form.FlowDirection = IsParentRightToLeft() ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
            }

            base.ShowDXDialog(form, rootElement, onDialogClosed, title, resizeMode, onOkClickDelegate);
        }
        #endregion

        #region Actions

        public void ConfigureDefaultBehavior()
        {
            var isRightToLeft = IsParentRightToLeft();

            BeginUpdate();
            try
            {
                DocumentViewDirection = isRightToLeft ? DocumentViewDirection.RightToLeft : DocumentViewDirection.LeftToRight;
                Options.Fields.HighlightMode = FieldsHighlightMode.Never;
                //Options.Fields.HighlightColor = Color.White;

                Options.Fields.UpdateFieldsOnPaste = true;
                Options.Fields.UpdateFieldsInTextBoxes = true;
                Options.Fields.UseCurrentCultureDateTimeFormat = false;
                Options.Fields.ThrowExceptionOnInvalidFormatSwitch = true;

                Options.Behavior.KeepTextHighlightingOnClearFormatting = true;
                Options.Behavior.ForeColorSource = RichEditBaseValueSource.Auto;
                Options.Behavior.FontSource = RichEditBaseValueSource.Auto;

                Options.Behavior.ShowPopupMenu = DocumentCapability.Enabled;
                Options.Behavior.RightToLeftTextDirection = DocumentCapability.Enabled;

                BehaviorOptions.Zooming = DocumentCapability.Enabled;
                BehaviorOptions.MinZoomFactor = 0.1f;
                BehaviorOptions.MaxZoomFactor = 5f;

                Options.Behavior.Zooming = DocumentCapability.Enabled;
                Options.Behavior.MinZoomFactor = 0.1f;
                Options.Behavior.MaxZoomFactor = 5f;

                Options.MailMerge.ViewMergedData = false;

                ConfigureDocumentDefaultProperties(isRightToLeft);
            }
            finally
            {
                EndUpdate();
            }
        }

        public void ConfigureDocumentDefaultProperties(bool isRightToLeft)
        {
            // Set default character formatting (font)
            Document.DefaultCharacterProperties.FontName = ThemeFontsHelper.TajawalFontName;
            Document.DefaultCharacterProperties.FontSize = 12;

            Document.DefaultTableProperties.RightToLeftTableLayout = isRightToLeft;

            // Set default paragraph formatting (layout)
            Document.DefaultParagraphProperties.LineSpacingType = DevExpress.XtraRichEdit.API.Native.ParagraphLineSpacing.Sesquialteral;
            Document.DefaultParagraphProperties.RightToLeft = isRightToLeft;
        }

        public void ExportToImage(Stream stream)
        {
            ExportToImage(stream, new ImageExportOptions());
        }

        public void ExportToImage(Stream stream, ImageExportOptions imageExportOptions)
        {
            this.ExecuteLinkActionWithDispose(delegate (LegacyPrintableComponentLink link)
            {
                link.ExportToImage(stream, imageExportOptions);
            });
        }

        public byte[] ExportToImage()
        {
            return StreamSerializationHelper.SerializeToByte(ExportToImage);
        }

        public new bool HandleException(Exception e)
        {
            return base.HandleException(e);
        }

        #endregion

        #region IRichDocumentEditControl
        public bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(this);

        public bool IsSelectionInMainDocument => !IsSelectionInTextBox && !IsSelectionInHeaderOrFooter && !IsSelectionInComment;

        float IRichEditorControl.DpiX => DpiX;

        float IRichEditorControl.DpiY => DpiY;

        DocumentModel IRichEditorControl.DocumentModel => DocumentModel;

        DocumentLayout IRichEditorControl.DocumentLayout => DocumentLayout;

        IRichReportDesigner IRichEditorControl.Owner => RichReportDesignerControl.GetDesigner(this);

        T IRichEditorControl.GetLayoutElement<T>(DocumentPosition position) => DocumentLayout.GetElement<T>(position);

        IRichEditDocumentServer IRichEditorControl.CreateDocumentServer() => CreateDocumentServer();

        Rectangle IRichEditorControl.GetBoundsFromPosition(DocumentPosition position)
        {
            return GetBoundsFromPosition(position);
        }

        Rectangle IRichEditorControl.GetLayoutLogicalBoundsFromPosition(DocumentPosition position)
        {
            return GetLayoutLogicalBoundsFromPosition(position);
        }

        DocumentPosition IRichEditorControl.GetPositionFromPoint(PointF clientPoint)
        {
            return GetPositionFromPoint(clientPoint);
        }

        System.Drawing.Point IRichEditorControl.PointToScreen(System.Drawing.Point clientPoint)
        {
            return PointToScreen(clientPoint);
        }

        System.Drawing.Point IRichEditorControl.GetPointFromPosition(DocumentPosition position, bool topBound)
        {
            Rectangle rectangle = Units.DocumentsToPixels(GetBoundsFromPosition(position), DpiX, DpiY);
            return PointToScreen(new System.Drawing.Point(rectangle.Left, topBound ? rectangle.Top : rectangle.Bottom));
        }

        public DocumentPosition GetDropPosition(System.Drawing.Point point)
        {
            // Slightly upward to avoid row boundary issues.
            //var documentPoint = new System.Drawing.PointF(
            //    Units.PixelsToDocumentsF((float)point.X, DpiX),
            //    Units.PixelsToDocumentsF((float)point.Y - 2, DpiY));

            //DocumentPosition position = GetPositionFromPoint(documentPoint);

            System.Drawing.Point documentPoint = Units.PixelsToDocuments(point, DpiX,DpiY);
            DocumentPosition position = GetPositionFromPoint(documentPoint);

            if (position != null)
                return position;
            
            return GetHitTestElementPosition(point);
        }

        public DocumentPosition GetHitTestElementPosition(System.Drawing.Point point)
        {
            PageLayoutPosition layoutPosition = ActiveView.GetDocumentLayoutPosition(point);

            if (layoutPosition == null)
                return null;

            LayoutPage page = DocumentLayout.GetPage(layoutPosition.PageIndex);
            if (page == null)
                return null;

            var hitTest = new HitTestManager(DocumentLayout);
            RichEditHitTestResult result = hitTest.HitTest(page, layoutPosition.Position);

            if (result?.LayoutElement is not RangedLayoutElement element)
                return null;

            var box = element as Box;

            // Use Start, not End, to avoid spilling into the next cell/row
            return Document.GetActiveSubDocument().GetPositionFromFixedRange(element.Range, box?.RightToLeft);
        }

        public DocumentPosition GetLayoutElementPosition(RangedLayoutElement element)
        {
            var subDocument = DocumentLayout.BeginUpdateDocument(element);
            try
            {
                var box = element as Box;
                return subDocument.GetPositionFromFixedRange(element.Range, box?.RightToLeft);
            }
            finally
            {
                DocumentLayout.EndUpdateDocument(subDocument);
            }
        }

        RichEditBrushBase IRichEditorControl.CreateBrush(System.Drawing.Color color)
        {
            return new RichEditBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
        }

        RichEditPenBase IRichEditorControl.CreatePen(System.Drawing.Color color, int thickness)
        {
            return new RichEditPen(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B),thickness);
        }

        async Task IRichEditorControl.ShowPreviewAsync()
        {
            await RichReportDesignerControl
                 .GetDesigner(this)
                 .ShowPrintPreviewAsync();
        }

        bool IRichEditorControl.CanShowPreview()
        {
            return RichReportDesignerControl
                    .GetDesigner(this)
                    .CanShowPreview();
        }

        async Task IRichEditorControl.SaveAsync()
        {
            await RichReportDesignerControl
                 .GetDesigner(this)
                 .SaveAsync();
        }

        bool IRichEditorControl.CanSave()
        {
            return RichReportDesignerControl
                    .GetDesigner(this)
                    .CanSave();
        }

        void IRichEditorControl.ShowCalculatedFieldsForm()
        {
            RichCalculatedFieldsEditorViewModel viewModel = RichCalculatedFieldsEditorViewModel.Create(this);

            RichCalculatedFieldsEditorForm view = new()
            {
                DataContext = viewModel
            };

            ShowDialog(view, null, RichEditFormNames.CalculatedFieldsEditor);
        }

        void IRichEditorControl.ShowCalculatedFieldModelForm(RichReportCalculatedFieldModel model, IRichReportCalculatedFieldModelOwner owner)
        {
            ExpressionEditorContext context = GetService<IRichEditExpressionContextFactoryService>().Create();

            RichCalculatedFieldForm view = new(context, model, owner);

            async void onDialogClosed(bool? dialogResult)
            {
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    await owner.SaveAsync(model);
                }
            }

            ShowDialog(view, this, onDialogClosed, RichEditFormNames.CalculatedField);
        }
        #endregion

        #region Privet Actions
        
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            XpfCommandProvider.Invalidate();
            XpfCommandProvider.Initialize();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            XpfCommandProvider.Invalidate();
        }

        protected bool IsParentRightToLeft()
        {
            return RichReportDesignerControl.GetDesigner(this)?.With(x => x as IRichReportDesignerControl)?.IsRightToLeft ?? FlowDirectionHelper.IsRightToLeft();
        }

        public void OnFieldHighlightingEnabledChanged()
        {
            if(!FieldHighlightingEnabled)
            {
                RichReportDesignerControl.GetDesigner(this).ClearFieldsHighlights();
            }
            else
            {
                RichReportDesignerControl.GetDesigner(this).HighlightsFields();
            }
        }
        #endregion
    }
}
