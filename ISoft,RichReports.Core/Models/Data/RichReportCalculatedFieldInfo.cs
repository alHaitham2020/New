using ISoft.RichReports.Core.Abstractions.Data;
using ISoft.RichReports.Core.Enums;
using System;

namespace ISoft.RichReports.Core.Models.Data
{
    public class RichReportCalculatedFieldInfo : ObjectBase, IRichReportCalculatedFieldInfo
    {
        public Guid? Id { get; set; }

        public int? ReportId { get; set; }

        public string FieldName { get; set; }

        public string DisplayName { get; set; }

        public string DataMember { get; set; }

        public string Expression { get; set; }

        public RichCalculatedFieldType? FieldType { get; set; }
    }
}
