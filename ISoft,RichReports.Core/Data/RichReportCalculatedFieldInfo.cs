using ISoft.RichReports.Core.Fields;
using ISoft.Utils.DataModel;
using System;

namespace ISoft.RichReports.Core.Data
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
