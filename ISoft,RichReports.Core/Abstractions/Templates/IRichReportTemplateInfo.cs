namespace ISoft.RichReports.Core.Abstractions.Templates
{
    public interface IRichReportTemplateInfo : IRichReportTemplateDataInfo
    {
        int? ReportId {  get;}

        string ReportName { get; }

        string ReportTitle { get;}
    }
}
