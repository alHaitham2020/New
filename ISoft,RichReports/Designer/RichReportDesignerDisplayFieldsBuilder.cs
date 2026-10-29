using ISoft.RichReports.Data.Fields;
using ISoft.RichReports.Designer.Fields;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.RichProperties;
using ISoft.RichReports.Editor.Services;
using ISoft.RichReports.Fields;
using ISoft.RichReports.UI;
using ISoft.Utils.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ISoft.RichReports.Designer
{
    public class RichReportDesignerDisplayFieldsBuilder : IRichReportDesignerDisplayFieldsBuilder
    {
        #region Constants
        private const string SystemFieldsGroupName = "SystemFields";
        private const string DocumentPropertiesGroupName = "DocumentProperties";
        private const string CustomPropertiesGroupName = "CustomProperties";
        private const string DocumentVariablesGroupName = "DocumentVariables";
        private const string CalculatedFieldsGroupName = "CalculatedFields";

        private const string SystemFieldsGroupDisplayName = "حقول أساسية";
        private const string DocumentPropertiesGroupDisplayName = "حقول التقرير";
        private const string CustomPropertiesGroupDisplayName = "حقول مخصصة";
        private const string DocumentVariablesGroupDisplayName = "حقول إضافية";
        private const string CalculatedFieldsGroupDisplayName = "حقول محسوبة";
        #endregion

        #region IRichReportDesignerDisplayFieldsBuilder Implementation

        public async Task<IRichDisplayFieldGroup[]> BuildAsync(IRichReport report)
            => await Task.Run(() => Build(report));

        public IRichDisplayFieldGroup[] Build(IRichReport report)
        {
            var groups = new List<RichDisplayFieldGroup>();

            if (report.TryGetService<IRichEditFieldsManagerService>(out var service))
            {
                TryAddGroup(groups, CreateSystemFieldGroup(service));
            }

            if (report.DocumentProperties.HasProprties) // Keep property name exactly matching your interface
            {
                TryAddGroup(groups, CreateDocumentFieldsGroup(report.DocumentProperties));
            }

            if (report.DocumentCustomProperties.HasProprties)
            {
                TryAddGroup(groups, CreateCustomFieldsGroup(report.DocumentCustomProperties));
            }

            if (report.DocumentVariablesProperties.HasProprties)
            {
                TryAddGroup(groups, CreateVariableFieldsGroup(report.DocumentVariablesProperties));
            }

            if (report.CalculatedFieldProperties.HasProprties)
            {
                TryAddGroup(groups, CreateCalculatedFieldsGroup(report.CalculatedFieldProperties));
            }

            if (report.DataFields.HasDataGroups)
            {
                groups.AddRange(CreateDataFieldGroups(report.DataFields));
            }

            groups.ApplyDistinctColorFamilies();

            return groups.ToArray();
        }

        #endregion

        #region Actions
        private void TryAddGroup(List<RichDisplayFieldGroup> groups, RichDisplayFieldGroup group)
        {
            EnsureVisible(group);
            groups.Add(group);
        }

        private void EnsureVisible(RichDisplayFieldGroup group)
        {
            if (!group.Fields.Any(x => x.IsVisible))
            {
                group.IsVisible = false;
            }
        }

        protected virtual RichDisplayFieldGroup CreateSystemFieldGroup(IRichEditFieldsManagerService provider)
        {
            RichDisplayFieldGroup group = new(
                name: SystemFieldsGroupName,
                displayName: SystemFieldsGroupDisplayName,
                path: SystemFieldsGroupName,
                groupCategory: RichFieldGroupCategory.SystemFields,
                parentGroup: null);
            
            TryAddSystemField(RichEditFieldType.PageField, group, provider);
            TryAddSystemField(RichEditFieldType.NumPagesField, group, provider);
            TryAddSystemField(RichEditFieldType.DateField, group, provider);
            TryAddSystemField(RichEditFieldType.TimeField, group, provider);
            TryAddSystemField(RichEditFieldType.PrintDateField, group, provider);

            return group;
        }

        protected virtual RichDisplayFieldGroup CreateDocumentFieldsGroup(IRichDocumentProperties properties)
        {
            var group = new RichDisplayFieldGroup(
                name: DocumentPropertiesGroupName,
                displayName: DocumentPropertiesGroupDisplayName,
                path: DocumentPropertiesGroupName,
                groupCategory: RichFieldGroupCategory.DocumentProperties,
                parentGroup: null);

            foreach (var property in properties.GetAllProperties())
            {
                group.AddDocumentPropertyField(property);
            }

            return group;
        }

        protected virtual RichDisplayFieldGroup CreateCustomFieldsGroup(IRichDocumentCustomProperties customProperties)
        {
            var group = new RichDisplayFieldGroup(
                name: CustomPropertiesGroupName,
                displayName: CustomPropertiesGroupDisplayName,
                path: CustomPropertiesGroupName,
                groupCategory: RichFieldGroupCategory.CustomProperties,
                parentGroup: null);

            foreach (var property in customProperties)
            {
                group.AddCustomPropertyField(property);
            }

            return group;
        }

        protected virtual RichDisplayFieldGroup CreateVariableFieldsGroup(IRichDocumentVariablesProperties variableProperties)
        {
            var group = new RichDisplayFieldGroup(
                name: DocumentVariablesGroupName,
                displayName: DocumentVariablesGroupDisplayName,
                path: DocumentVariablesGroupName,
                groupCategory: RichFieldGroupCategory.DocumentVariables,
                parentGroup: null);

            foreach (var property in variableProperties)
            {
                group.AddDocumentVaribaleField(property);
            }

            return group;
        }

        protected virtual RichDisplayFieldGroup CreateCalculatedFieldsGroup(IRichCalculatedFieldProperties calculatedProperties)
        {
            var group = new RichDisplayFieldGroup(
                name: CalculatedFieldsGroupName,
                displayName: CalculatedFieldsGroupDisplayName,
                path: CalculatedFieldsGroupName,
                groupCategory: RichFieldGroupCategory.CalculatedFields,
                parentGroup: null);

            foreach (var property in calculatedProperties)
            {
                group.AddCalculatedPropertyField(property);
            }

            return group;
        }

        protected virtual IEnumerable<RichDisplayFieldGroup> CreateDataFieldGroups(IRichDataFieldGroupCollection dataGroups)
        {
            var groups = new List<RichDisplayFieldGroup>();

            foreach (var dataGroup in dataGroups)
            {
                var displayGroup = RichDisplayFieldGroup.FromDataGroup(dataGroup, null);

                foreach (var dataField in dataGroup.Fields)
                {
                    displayGroup.AddField(RichDisplayField.FromDataField(dataField, displayGroup));
                }

                EnsureVisible(displayGroup);
                groups.Add(displayGroup);
            }

            return groups;
        }

        protected void TryAddSystemField(RichEditFieldType fieldType, RichDisplayFieldGroup group, IRichEditFieldsManagerService service)
        {
            if (!service.TryGetFieldInfo(fieldType, out var fieldInfo))
            {
                throw new InvalidOperationException($"The target fieldType {fieldType} does not have info.");
            }

            group.AddSystemField(
                fieldName: fieldInfo.FieldName,
                displayName: service.GetFieldDisplayName(fieldType),
                dataType: fieldInfo.Type,
                fieldType: fieldInfo.FieldType,
                format: fieldInfo.DefaultFormat,
                isVisible: true);
        }

        #endregion
    }
}
