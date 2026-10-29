using ISoft.RichReports.Data.Fields;
using ISoft.RichReports.Fields;
using System;
using System.Drawing;

namespace ISoft.RichReports.Designer.Fields
{
    public class RichDisplayField : RichFieldNodeBase, IRichDisplayField
    {
        #region Fields
        private readonly RichEditFieldType _fieldType;
        private readonly RichEditFieldKey _fieldKey;
        #endregion

        #region Proprties
        public RichEditFieldType FieldType => _fieldType;

        public RichEditFieldKey FieldKey => _fieldKey;

        public bool IsNestedPictureField => FieldType != RichEditFieldType.IncludePictureField && IsImageField;

        public new IRichDisplayFieldGroup ParentGroup
        {
            get
            {
                return base.ParentGroup as IRichDisplayFieldGroup;
            }
            set
            {
                base.ParentGroup = value;
            }
        }

        Color IRichFieldHighlightInfo.BackgroundColor => ParentGroup.BackgroundColor;

        Color IRichFieldHighlightInfo.FontColor => ParentGroup.FontColor;

        public bool PreventMultipleInsertion { get; set; }

        #endregion

        public RichDisplayField(IRichDataField dataField, IRichDisplayFieldGroup parentGroup)
        {
            _fieldType = RichEditFieldType.MergeField;
            _fieldKey = RichEditFieldKey.CreateFromType(_fieldType, dataField.FieldProperty);

            Name = dataField.Name;
            DisplayName = dataField.DisplayName;
            Path = dataField.Path;
            FieldProperty = dataField.FieldProperty;
            DataMember = dataField.DataMember;
            DataTypeCategory = dataField.DataTypeCategory;
            DataType = dataField.DataType;
            DataTypeKind = dataField.DataTypeKind;
            ImageSchemeType = dataField.ImageSchemeType;
            FieldFormat = dataField.FieldFormat;
            IsDetailField = dataField.IsDetailField;
            IsVisible = dataField.IsVisible;
            NullValue = dataField.NullValue;
            ParentGroup = parentGroup;
        }

        public RichDisplayField(
            RichEditFieldType fieldType,
            string name,
            string displayName,
            string path,
            string fieldProperty,
            Type dataType,
            IRichDisplayFieldGroup parentGroup,
            RichFieldFormat? format = null,
            ImageUriSchemeType? imageUriSchemeType = null,
            bool? isVisible = null)
        {
            _fieldType = fieldType;
            _fieldKey = RichEditFieldKey.CreateFromType(fieldType, fieldProperty);

            Name = name;
            DisplayName = displayName;
            Path = path;
            FieldProperty = fieldProperty;
            DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
            ParentGroup = parentGroup ?? throw new ArgumentNullException(nameof(parentGroup));
            FieldFormat = format;
            ImageSchemeType = imageUriSchemeType ?? ImageUriSchemeType.None;
            DataTypeKind = dataType.GetUnderlyingTypeOrType().GetDataTypeKind();
            DataTypeCategory = dataType.GetUnderlyingTypeOrType().GetFieldCategory(ImageSchemeType);
            IsVisible = isVisible ?? true;
        }

        public static RichDisplayField FromDataField(IRichDataField dataField, IRichDisplayFieldGroup parentGroup)
        {
           return new RichDisplayField(dataField, parentGroup);
        }

        public virtual object CreateFieldOptions()
        {
            return RichDisplayFieldOptionsFactory.CreateOptions(this);
        }
    }
}
