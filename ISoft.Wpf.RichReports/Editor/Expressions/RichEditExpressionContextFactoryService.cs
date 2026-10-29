using DevExpress.Data.Controls.ExpressionEditor;
using DevExpress.Xpf.ExpressionEditor;
using ISoft.RichReportDesigner.Core.Constants;
using ISoft.RichReportDesigner.Core.Fields.Data;
using ISoft.RichReportDesigner.Core.RichEdit;
using ISoft.RichReportDesigner.Core.RichEdit.Expression;
using ISoft.RichReportDesigner.Core.RichEdit.Properties;
using ISoft.RichReportDesigner.Core.RichEdit.Services;
using ISoft.Utils.Services;
using System.Linq;

namespace ISoft.RichReportDesigner.UI.RichEdit.ExpressionEditor
{
    /// <summary>
    /// Factory responsible for creating <see cref="ExpressionEditorContext"/> instances
    /// used by the rich text expression editor.
    /// </summary>
    public class RichEditExpressionContextFactoryService : IRichEditExpressionContextFactoryService
    {
        private readonly IRichEditorControl _editControl;

        public RichEditExpressionContextFactoryService(IRichEditorControl editControl)
        {
            _editControl = editControl;
        }

        /// <summary>
        /// Creates an expression editor context populated with data field columns
        /// and calculated field columns from the associated edit control's services.
        /// </summary>
        public ExpressionEditorContext Create()
        {
            var context = ExpressionEditorContextHelper.GetContext();

            PopulateDataColumns(context);
            PopulateCalculatedFieldsColumns(context);

            return context;
        }

        /// <summary>
        /// Adds columns for data fields supplied by the edit control's
        /// <see cref="IRichDataFieldsProvider"/>, if one is registered.
        /// </summary>
        protected virtual void PopulateDataColumns(ExpressionEditorContext context)
        {
            // Skip silently when the service isn't available.
            if (_editControl.GetService<IRichDataFieldsProvider>() is not { } dataFieldsProvider)
                return;

            var dataColumns = dataFieldsProvider.DataFields.SelectMany(group => group.Fields, (_, field) => CreateDataColumn(field));

            context.Columns.AddRange(dataColumns);
        }

        /// <summary>
        /// Adds columns for calculated fields supplied by the edit control's
        /// <see cref="IRichCalculatedFieldManagerService"/>, if one is registered.
        /// </summary>
        protected virtual void PopulateCalculatedFieldsColumns(ExpressionEditorContext context)
        {
            // Skip silently when the service isn't available.
            if (_editControl.GetService<IRichCalculatedFieldManagerService>() is not { } calculatedFieldService)
                return;

            var calculatedColumns = calculatedFieldService.CalculatedFieldProperties
                                                           .Select(CreateCalculatedFieldColumn);

            context.Columns.AddRange(calculatedColumns);
        }

        /// <summary>
        /// Maps a single data field to an editor column info entry.
        /// </summary>
        protected static RichExpressionEditorColumnInfo CreateDataColumn(IRichDataField dataField)
        {
            return new(dataField.ParentGroup.DisplayName)
            {
                ColumnType = RichExpressionEditorColumnType.DataColumn,
                Name = dataField.FieldDataName,
                Description = dataField.DisplayName,
                DataMember = dataField.DataMember,
                IsDetailColumn = dataField.IsDetailField,
                Type = dataField.GetActualDataType(),
            };
        }

        /// <summary>
        /// Maps a single calculated field property to an editor column info entry.
        /// </summary>
        protected static RichExpressionEditorColumnInfo CreateCalculatedFieldColumn(IRichCalculatedFieldProperty fieldProperty)
        {
            return new(RichExpressionEditorCategories.CalculatedFields)
            {
                ColumnType = RichExpressionEditorColumnType.CalculatedColumn,
                Name = fieldProperty.Name,
                Description = fieldProperty.DisplayName,
                DataMember = fieldProperty.DataMember,
                IsDetailColumn = fieldProperty.HasValidDataMember,
                Type = fieldProperty.PropertyType,
            };
        }
    }
}
