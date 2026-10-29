using ISoft.RichReports.Data.Fields;
using ISoft.RichReports.Fields;
using System;
using System.Drawing;

namespace ISoft.RichReports.Designer.Fields
{
    public class RichDisplayFieldGroup : RichFieldGroupBase, IRichDisplayFieldGroup, IDrawingTextPaletteColorsOwner
    {
        #region Fields
        private readonly RichDisplayFieldCollection _fields;
        private Color _fontColor;
        private Color _backgroundColor;
        #endregion

        #region Proprites
        public IRichDisplayFieldsCollection Fields => _fields;

        public Color FontColor
        {
            get => _fontColor;
            set => _fontColor = value;
        }

        public Color BackgroundColor
        {
            get => _backgroundColor;
            set => _backgroundColor = value;
        }

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

        #endregion

        public RichDisplayFieldGroup(
            string name,
            string displayName,
            string path,
            RichFieldGroupCategory groupCategory,
            Type dataType = null,
            IRichDisplayFieldGroup parentGroup = null,
            bool? isVisible = null)
            : base(name, displayName, path, groupCategory, dataType, parentGroup, isVisible)
        {
            _fields = new();
        }

        #region Actions
        public static RichDisplayFieldGroup FromDataGroup(IRichDataFieldGroup dataGroup, IRichDisplayFieldGroup parentGroup)
        {
            return new RichDisplayFieldGroup(
                name: dataGroup.Name,
                displayName: dataGroup.DisplayName,
                path: dataGroup.Path,
                groupCategory: RichFieldGroupCategory.DataFields,
                dataType: dataGroup.DataType,
                parentGroup: parentGroup,
                isVisible: dataGroup.IsVisible);
        }

        internal void AddField(IRichDisplayField field)
        {
            _fields.Add(field);
        }

        internal void AddOrUpdateField(IRichDisplayField field)
        {
            _fields.AddOrUpdate(field);
        }

        internal void RemoveField(IRichDisplayField field)
        {
            if (field == null) return;

            _fields.RemoveByKey(field.FieldKey);
        }

        internal void AddCalculatedPropertyField(IRichCalculatedFieldProperty property)
        {
            var displayField = CreatePropertyField(
                 fieldType: RichEditFieldType.ExpEditCalculatedField,
                 name: property.Name,
                 displayName: property.DisplayName,
                 dataType: property.PropertyType,
                 imageUriSchemeType: null,
                 format: property.Format,
                 isVisible: property.IsVisible);

            if (property.HasValidDataMember)
            {
                displayField.IsDetailField = true;
                displayField.DataMember = property.DataMember;
            }
            else
            {
                displayField.IsDetailField = false;
                displayField.DataMember = string.Empty;
            }

            AddOrUpdateField(displayField);
        }

        internal void AddDocumentPropertyField(IRichDocumentProperty property)
        {
            AddPropertyField(
                fieldType: RichEditFieldType.DocPropertyField,
                name: property.Name,
                displayName: property.DisplayName,
                dataType: property.PropertyType,
                imageUriSchemeType: null,
                format: property.Format,
                isVisible: property.IsVisible);
        }

        internal void AddCustomPropertyField(IRichDocumentCustomProperty property)
        {
            AddPropertyField(
                fieldType: RichEditFieldType.CustomPropertyField,
                name: property.Name,
                displayName: property.DisplayName,
                dataType: property.PropertyType,
                imageUriSchemeType: property.ImageUriSchemeType,
                format: property.Format,
                isVisible: property.IsVisible);
        }

        internal void AddDocumentVaribaleField(IRichDocumentVariableProperty property)
        {
            AddPropertyField(
                fieldType: RichEditFieldType.DocVariableField,
                name: property.Name,
                displayName: property.DisplayName,
                dataType: property.PropertyType,
                imageUriSchemeType: property.ImageUriSchemeType,
                format: property.Format,
                isVisible: property.IsVisible);
        }

        internal void AddSystemField(
           string fieldName,
           string displayName,
           Type dataType,
           RichEditFieldType fieldType,
           RichFieldFormat? format = null,
           bool? isVisible = null)
        {
            AddField(CreateField(
                fieldType: fieldType,
                name: fieldName,
                displayName: displayName,
                fieldProperty: null, // system fields dosnot support property
                dataType: dataType,
                imageUriSchemeType: null,
                format: format,
                isVisible: isVisible));
        }

        private void AddPropertyField(
         RichEditFieldType fieldType,
         string name,
         string displayName,
         Type dataType,
         ImageUriSchemeType? imageUriSchemeType = null,
         RichFieldFormat? format = null,
         bool? isVisible = null)
        {
            AddOrUpdateField(CreatePropertyField(
                fieldType: fieldType,
                name: name,
                displayName: displayName,
                dataType: dataType,
                imageUriSchemeType: imageUriSchemeType,
                format: format,
                isVisible: isVisible));
        }

        private RichDisplayField CreatePropertyField(
         RichEditFieldType fieldType,
         string name,
         string displayName,
         Type dataType,
         ImageUriSchemeType? imageUriSchemeType = null,
         RichFieldFormat? format = null,
         bool? isVisible = null)
        {
            return CreateField(
                    fieldType: fieldType,
                    name: name,
                    displayName: displayName,
                    fieldProperty: name, // the propertyName is the fieldProperty
                    dataType: dataType,
                    imageUriSchemeType: imageUriSchemeType,
                    format: format,
                    isVisible: isVisible);
        }

        protected RichDisplayField CreateField(
            RichEditFieldType fieldType,
            string name,
            string displayName,
            string fieldProperty,
            Type dataType,
            ImageUriSchemeType? imageUriSchemeType = null,
            RichFieldFormat? format = null,
            bool? isVisible = null)
        {
            return new RichDisplayField(fieldType: fieldType,
                                        name: name,
                                        displayName: displayName,
                                        path: BuildPath(name),
                                        fieldProperty: fieldProperty,
                                        dataType: dataType,
                                        parentGroup: this,
                                        format: format,
                                        imageUriSchemeType: imageUriSchemeType,
                                        isVisible: isVisible);

        }

        protected string BuildPath(string name)
        {
            return RichFieldDataPathHelper.BuildPath(Path, name);
        }

        #endregion
    }
}
