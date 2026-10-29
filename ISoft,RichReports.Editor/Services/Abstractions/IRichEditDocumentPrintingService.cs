using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditDocumentPrintingService
    {
        void Export(IRichEditDocumentServer printable, ExportFormat format, string defaultFileName = null);

        void Print(IRichEditDocumentServer printable);

        void ShowPrintDialog(IRichEditDocumentServer printable);

        void ShowPrintPreview(IBasePrintable printable, string title = null);
    }
}
