using ISoft.RichReports.Models.Reports;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.Printing
{
    public class RichReportPrintManager : IRichReportPrintManager
    {
        private readonly IRichReport _report;

        public RichReportPrintManager(IRichReport report)
        {
            _report = report ?? throw new ArgumentNullException(nameof(report));
        }

        public async Task ShowPrintPreviewAsync(IRichReportTemplate templateSource, string title = null)
        {
            await ExecutePrintOperationAsync(
                templateSource,
                async server => server.ShowPrintPreview(title),
                "جاري تجهيز المعاينة ...");
        }

        public async Task ShowPrintDialogAsync(IRichReportTemplate templateSource)
        {
            await ExecutePrintOperationAsync(
                templateSource,
                server => server.ShowPrintDialog(),
                "جاري تجهيز مربع حوار الطباعة ..."
            );
        }

        public async Task PrintAsync(IRichReportTemplate templateSource)
        {
            await ExecutePrintOperationAsync(
                templateSource,
                server => server.Print(),
                "جاري الطباعة ..."
            );
        }

        public async Task ExportAsync(IRichReportTemplate templateSource, ExportFormat format, string defaultFileName = null)
        {
            await ExecutePrintOperationAsync(
                templateSource,
                server => server.Export(format, defaultFileName),
                "جاري التصدير ..."
            );
        }

        private async Task ExecutePrintOperationAsync(
            IRichReportTemplate templateSource,
            Action<IRichDocumentPreviewServer> operation,
            string initialMessage)
        {
            ArgumentGuardHelper.ArgumentNotNull(templateSource, nameof(templateSource));

            object dataSource = _report.DataSource?.Data;
            ArgumentGuardHelper.ArgumentNotNull(dataSource, nameof(dataSource));

            var waitIndicationService = _report.GetService<IRichWaitIndicationService>();

            await waitIndicationService.BeginAsync(initialMessage);

            try
            {
                using var server = RichDocumentPreviewServer.Create(_report);

                await waitIndicationService.UpdateAsync("جاري تحميل نمط الطباعة ...");
                await server.LoadTemplateAsync(templateSource);

                await waitIndicationService.UpdateAsync("جاري جلب البيانات ...");
                await server.FillDataAsync(dataSource);
                await waitIndicationService.EndAsync();

                operation(server);
            }
            catch
            {
                await waitIndicationService.EndAsync();
                throw;
            }
        }

        public void Dispose()
        {
            
        }
    }
}
