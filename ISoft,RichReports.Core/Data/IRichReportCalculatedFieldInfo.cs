using ISoft.RichReports.Core.Fields;
using System;

namespace ISoft.RichReports.Core.Data
{
    public interface IRichReportCalculatedFieldInfo
    {
        Guid? Id { get; }

        string FieldName { get; }

        int? ReportId { get; }

        string DataMember { get; }

        string DisplayName { get; }

        string Expression { get; }

        RichCalculatedFieldType? FieldType { get;}
    }
}
