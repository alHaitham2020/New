using ISoft.Wpf.RichReports.Models;

namespace ISoft.Wpf.RichReports.Helpers
{
    /// <summary>
    /// Static helper for WPF report document operations
    /// </summary>
    public static class XpfRichReportHelper
    {
        private const string TemplatesEditorDialogTitle = " إختيار نمط الطباعة - {0}";

        private static readonly Func<IXpfRichReport, RichReportDesignerDocumentViewModel>
            DesignerFactory = ViewModelSource.Factory<IXpfRichReport, RichReportDesignerDocumentViewModel>(
                p => new RichReportDesignerDocumentViewModel(p));

        private static readonly Func<IXpfRichReport, RichReportTemplatesEditorDialogViewModel>
            EditorFactory = ViewModelSource.Factory<IXpfRichReport, RichReportTemplatesEditorDialogViewModel>(
                r => new RichReportTemplatesEditorDialogViewModel(r));

        public static void ShowReportViewer(IXpfRichReport report, string title)
        {
            var viewModel = EditorFactory(report);

            ShowDocument<RichReportPreviewDocumentView>(viewModel, title);
        }

        public static void ShowTemplatesEditorDialog(IXpfRichReport report, string title)
        {
            var viewModel = EditorFactory(report);
            var docTitle = string.Format(TemplatesEditorDialogTitle, title);
            ShowDocument<RichReportTemplatesEditorDialogView>(viewModel, docTitle);
        }

        public static void ShowReportDesignerDocument(IXpfRichReport report)
        {
            var owner = WindowsUIHelper.GetLastWindow(true);
            var viewModel = DesignerFactory(report);
            ShowDocument<RichReportDesignerDocumentView>(viewModel, report.DesignerTemplate.GetDesignerTitle(), owner);
        }

        private static void ShowDocument<TView>(
            object viewModel,
            string title,
            object parentViewModel = null)
        {
            DomainServiceContainerHelper
                .GetService<IDomainDocumentsManagerService>()
                .ShowWindowedDocument(
                    documentType: typeof(TView).FullName,
                    documentTitle: title,
                    documentShowMode: WindowShowMode.Dialog,
                    viewModel: viewModel,
                    parameter: null,
                    parentViewModel: parentViewModel);
        }
    }
}
