using System;

namespace ISoft.RichReports.Core.Templates
{
    public class RichReportTemplateDataInfo : RichReportTemplateData, IRichReportTemplateDataInfo
    {
        public Guid? OwnerId { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsShared { get; set; }

        public string OwnerName { get; set; }
    }

}
