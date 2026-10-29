using System;

namespace ISoft.RichReports.Models
{
    public class RichReportCalculatedFieldModel : ObjectBase, IRichReportCalculatedFieldInfo
    { 
        public static readonly string FieldNamePrefix = "CalculatedField";
        public static readonly string DisplayNamePrefix = "حقل";

        private string _fieldName;
        private string _displayName;
        private string _expression;
        private RichCalculatedFieldType? _fieldType = RichCalculatedFieldType.None;

        public Guid? Id { get; set; }

        public int? ReportId { get; set; }

        public RichCalculatedFieldType? FieldType
        {
            get => _fieldType;
            set => _fieldType = value;
        }

        public string FieldName
        {
            get
            {
                return _fieldName;
            }
            set
            {
                _fieldName = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public string DataMember { get; set; }

        public string Expression
        {
            get
            {
                return _expression;
            }
            set
            {
                _expression = value;
            }
        }

        public bool IsDetail => !string.IsNullOrWhiteSpace(DataMember);

        public RichReportCalculatedFieldModel(IRichReportCalculatedFieldInfo info)
        {
            ArgumentGuardHelper.ArgumentNotNull(info, nameof(info));

            Id = info.Id;
            FieldName = info.FieldName;
            DisplayName = info.DisplayName;
            DataMember = info.DataMember;
            Expression = info.Expression;
            ReportId = info.ReportId;
            FieldType = info.FieldType;
        }

        public RichReportCalculatedFieldModel()
        {

        }
    }
}
