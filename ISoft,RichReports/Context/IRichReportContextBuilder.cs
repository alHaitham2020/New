using ISoft.RichReports.Data.Sources;
using ISoft.RichReports.Models.Reports;

namespace ISoft.RichReports.Context
{
    public interface IRichReportContextBuilder
    {
        IRichReportContext Build();

        IRichReportContextBuilder WithReportInfo(IReportInfo reportInfo);

        IRichReportContextBuilder WithDataSource(IRichReportDataSource dataSource);

        IRichReportContextBuilder WithDefaultTemplateInfoProvider(IRichDefaultTemplateInfoProvider provider);

        IRichReportContextBuilder WithPermissionsInfo(IRichReportPrintPermissionInfo permissions);

        IRichReportContextBuilder WithProperties(IRichReportProperties properties);

        IRichReportContextBuilder WithCustomProperties(IRichDocumentCustomProperties properties);

        IRichReportContextBuilder WithTemplateInfoProvider(IRichReportTemplateInfoProvider provider);

        IRichReportContextBuilder WithTemplateStorage(IRichReportTemplateStorage storage);

        IRichReportContextBuilder WithDocumentProperties(IRichDocumentProperties documentProperties);

        IRichReportContextBuilder WithVariableProperties(IRichDocumentVariablesProperties variableProperties);

        IRichReportContextBuilder WithCalculatedFieldInfoProvider(IRichReportCalculatedFieldInfoProvider provider);
    }
}
