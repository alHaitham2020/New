using ISoft.RichReports.Core.Abstractions.Templates;
using System;

namespace ISoft.RichReports.Core.Models.Templates
{
    public class RichReportTemplateInfo : ObjectBase, IRichReportTemplateInfo
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }

        public byte[] Thumbnail { get; set; }

        public string Version { get; set; }

        public Guid? OwnerId { get; set; }

        public string OwnerName { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool? IsShared { get; set; }

        public int? ReportId { get; set; }

        public string ReportName { get; set; }

        public string ReportTitle { get; set; }
    }
}
