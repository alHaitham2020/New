using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.Printing
{
    public interface IRichReportPrintManager : IDisposable
    {
        Task PrintAsync(IRichReportTemplate templateSource);

        Task ShowPrintDialogAsync(IRichReportTemplate templateSource);

        Task ShowPrintPreviewAsync(IRichReportTemplate templateSource, string title = null);

        Task ExportAsync(IRichReportTemplate templateSource, ExportFormat format, string defaultFileName = null);
    }
}
