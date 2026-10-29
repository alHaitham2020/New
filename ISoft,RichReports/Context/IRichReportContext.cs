using ISoft.Printing.Shared.Data;
using ISoft.RichReports.Core.Client;
using ISoft.RichReports.Data.DataSources;
using ISoft.RichReports.Editor.RichProperties;

namespace ISoft.RichReports.UI
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
