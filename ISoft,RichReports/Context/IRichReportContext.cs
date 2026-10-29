using ISoft.RichReports.Data.Sources;
using ISoft.RichReports.Models.Reports;

namespace ISoft.RichReports.Context
{
    /// <summary>
    /// Report context containing all required dependencies
    /// </summary>
    public interface IRichReportContext
    {
        IReportInfo ReportInfo { get; }

        IRichReportDataSource DataSource { get; }

        IRichReportTemplateStorage TemplateStorage { get; }

        IRichReportCalculatedFieldStorage CalculatedFieldStorage { get; }

        IRichReportPrintPermissionInfo PermissionInfo { get; }

        IRichReportProperties Properties { get; }

        IRichDocumentProperties DocumentProperties { get; }

        IRichDocumentCustomProperties DocumentCustomProperties { get; }

        IRichDocumentVariablesProperties DocumentVariableProperties { get; }

        IRichReportTemplateInfoProvider TemplateInfoProvider { get; }

        IRichDefaultTemplateInfoProvider DefaultTemplateInfoProvider { get; }
    }
}
