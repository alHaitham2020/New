using System;

namespace ISoft.RichReports.Core.Abstractions.Templates
{
    public interface IRichReportTemplateDataInfo : IRichReportTemplateData
    {
        Guid? OwnerId { get; }

        string OwnerName { get; }

        DateTime? ModifiedDate { get; }

        bool? IsShared { get; }
    }
}
