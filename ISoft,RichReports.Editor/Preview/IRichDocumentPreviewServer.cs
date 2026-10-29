using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using ISoft.RichReports.Core.Templates;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Preview
{
    /// <summary>
    /// Interface for template processor
    /// </summary>
    public interface IRichDocumentPreviewServer : IDisposable
    {
        Task LoadTemplateAsync(IRichReportTemplate templateSource, DocumentFormat? format = null);

        Task FillDataAsync(object dataSource, MailMergeOptions options = null);

        byte[] SaveTemplate(DocumentFormat? format = null);

        Task<byte[]> SaveTemplateAsync(DocumentFormat? format = null);

        Task SaveTemplateAsync(string fileName, DocumentFormat? format = null);

        Task SaveTemplateAsync(Stream stream, DocumentFormat? format = null);

        void ShowPrintPreview(string title = null);

        void ShowPrintDialog();

        void Print();

        void Export(ExportFormat format, string defaultFileName = null);
    }
}
