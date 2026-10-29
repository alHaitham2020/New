using ISoft.Utils.DataModel;
using System;

namespace ISoft.RichReports.Core.Templates
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
