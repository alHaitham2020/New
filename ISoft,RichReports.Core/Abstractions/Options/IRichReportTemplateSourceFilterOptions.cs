using ISoft.RichReports.Core.Enums;

namespace ISoft.RichReports.Core.Abstractions.Options
{
    public interface IRichReportTemplateSourceFilterOptions
    {
        IReportInfo ReportInfo { get;}

        RichReportTemplateSourceType SourceType { get;}

        bool IsValid();

        void Validate();
    }
}
