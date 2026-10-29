using ISoft.Printing.Shared.Data;

namespace ISoft.RichReports.Core.Client
{
    public interface IRichReportTemplateSourceFilterOptions
    {
        IReportInfo ReportInfo { get;}

        RichReportTemplateSourceType SourceType { get;}

        bool IsValid();

        void Validate();
    }
}
