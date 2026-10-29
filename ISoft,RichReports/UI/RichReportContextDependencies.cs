using ISoft.Printing.Shared.Data;
using ISoft.RichReports.Core.Client;
using ISoft.RichReports.Data.DataSources;
using ISoft.RichReports.Editor.RichProperties;

namespace ISoft.RichReports.UI
{
    public class RichReportContextDependencies
    {
        public IReportInfo ReportInfo { get; set; }

        public IRichReportDataSource DataSource { get; set; }

        public IRichReportTemplateStorage TemplateStorage { get; set; }

        public IRichReportPrintPermissionInfo PermissionInfo { get; set; }

        public IRichReportProperties Properties { get; set; }

        public IRichDocumentProperties DocumentProperties { get; set; }

        public IRichDocumentCustomProperties DocumentCustomProperties { get; set; }

        public IRichDocumentVariablesProperties DocumentVariableProperties { get; set; }

        public IRichReportTemplateInfoProvider TemplateInfoProvider { get; set; }

        public IRichDefaultTemplateInfoProvider DefaultTemplateInfoProvider { get; set; }

        public IRichReportCalculatedFieldInfoProvider CalculatedFieldInfoProvider { get; set; }
    }
}
