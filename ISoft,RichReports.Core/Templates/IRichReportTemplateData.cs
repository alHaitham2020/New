using System;

namespace ISoft.RichReports.Core.Templates
{
    public interface IRichReportTemplateData : ICloneable
    {
        Guid? Id { get; }

        string Name { get; }

        byte[] Content { get; }

        byte[] Thumbnail { get; }

        string Version { get; }
    }
}
