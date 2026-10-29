using ISoft.RichReports.Data.Fields;

namespace ISoft.RichReports.Fields
{
    public abstract class RichFieldNodeBase : RichNodeBase, IRichFieldNode
    {
        #region Feilds
        private string _fieldProperty;
        private bool _isDetailField;
        private string _dataMember;
        private RichFieldFormat? _fieldFormat;
        private object _nullValue;
        private DataTypeKind _dataTypeKind = DataTypeKind.None;
        private RichFieldDataTypeCategory _dataTypeCategory = RichFieldDataTypeCategory.None;
        private ImageUriSchemeType _imageSchemeType = ImageUriSchemeType.None;
        #endregion

        #region Proprties
        public DataTypeKind DataTypeKind
        {
            get => _dataTypeKind;
            set { _dataTypeKind = value; }
        }

        public RichFieldDataTypeCategory DataTypeCategory
        {
            get => _dataTypeCategory;
            set { _dataTypeCategory = value; }
        }

        public string FieldProperty
        {
            get => _fieldProperty;
            set { _fieldProperty = value; }
        }

        public RichFieldFormat? FieldFormat
        {
            get => _fieldFormat;
            set => _fieldFormat = value;
        }

        public object NullValue
        {
            get => _nullValue;
            set { _nullValue = value; }
        }

        public bool IsDetailField
        {
            get => _isDetailField;
            set { _isDetailField = value; }
        }

        public string DataMember
        {
            get => _dataMember;
            set => _dataMember = value;
        }

        public ImageUriSchemeType ImageSchemeType
        {
            get => _imageSchemeType;
            set => _imageSchemeType = value;
        }

        public virtual bool IsImageField => ImageSchemeType != ImageUriSchemeType.None;

        public bool HasFormat => !RichFieldFormat.IsNullOrEmpty(FieldFormat);

        public override bool IsGroup => false;

        #endregion
    }
}
