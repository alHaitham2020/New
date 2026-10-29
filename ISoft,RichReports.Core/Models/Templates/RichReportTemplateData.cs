using ISoft.RichReports.Core.Abstractions.Templates;
using System;

namespace ISoft.RichReports.Core.Models.Templates
{
    public class RichReportTemplateData : ObjectBase, IRichReportTemplateData
    {
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }

        public byte[] Thumbnail { get; set; }

        public string Version { get; set; }
    }
}
