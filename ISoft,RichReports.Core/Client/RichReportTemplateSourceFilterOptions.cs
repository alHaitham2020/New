using ISoft.Printing.Shared.Data;
using System;

namespace ISoft.RichReports.Core.Client
{
    public class RichReportTemplateSourceFilterOptions : IRichReportTemplateSourceFilterOptions
    {
        public RichReportTemplateSourceType SourceType { get; set; } = RichReportTemplateSourceType.AllTemplates;

        public IReportInfo ReportInfo { get; set; }

        public bool IsValid()
        {
            if (SourceType != RichReportTemplateSourceType.AllTemplates)
            {
                return ReportInfo != null;
            }

            return true;
        }

        public void Validate()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException("ReportInfo can't be null");
            }
        }

        public static RichReportTemplateSourceFilterOptions Create(IReportInfo reportInfo, RichReportTemplateSourceType sourceType)
        {
            return new RichReportTemplateSourceFilterOptions
            {
                ReportInfo = reportInfo,
                SourceType = sourceType
            };
        }
    }
}
