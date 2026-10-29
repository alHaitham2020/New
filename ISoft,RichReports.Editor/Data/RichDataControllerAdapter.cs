using ISoft.RichReports.Editor.Services.Abstractions;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ISoft.RichReports.Editor.Data
{
    public class RichDataControllerAdapter : RichEditDataControllerAdapter, IDataControllerData2
    {
        private readonly IRichEditCalculatedFieldManagerService calculatedFieldManagerService;

        protected new RichEditDataController DataController => base.DataController as RichEditDataController;

        public RichDataControllerAdapter(BaseGridController dataController, IRichEditCalculatedFieldManagerService fieldManagerService) : base(dataController)
        {
            calculatedFieldManagerService = fieldManagerService;
        }

        #region Actions
        public static RichDataControllerAdapter CreateDefault(IRichEditCalculatedFieldManagerService fieldManagerService)
        {
            return new RichDataControllerAdapter(RichEditDataController.CreateDefault(),fieldManagerService);
        }

        protected PropertyDescriptorCollection Properties => DataController.Helper.DescriptorCollection;

        protected ExpressionEvaluator CreateExpressionEvaluator(string expression)
        {
            return DataController.CreateExpressionEvaluator(CriteriaOperator.TryParse(expression), setDataAccess: true, out _);
        }

        public IEnumerable<DevExpress.Data.Controls.ExpressionEditor.ColumnInfo> GetExpressionColumns()
        {
            return DataController.Columns.Select(x =>
            new DevExpress.Data.Controls.ExpressionEditor.ColumnInfo()
            {
                Name = x.Name,
                Type = x.Type,
                Category = "Fields",
            }).ToArray();
        }

        public IEnumerable<IDataColumnInfo> GetDataColumns() => DataController.Columns;

        public IEnumerable<IDataColumnInfo> GetDetailDataColumns() => DataController.DetailColumns;

        public DataColumnInfo GetDataColumn(string fieldName) => DataController.FindColumn(fieldName);

        public object GetCurrentRowValue(string fieldName) => DataController.GetCurrentRowValue(fieldName);

        public object GetFieldValueFromDataSource(string fieldName)
        {
            var currentRow = GetCurrentRow();
            if (currentRow == null) return null;
            return currentRow.GetPropertyValueFromPath(fieldName);
        }

        public object GetCurrentRowEvaluatedValue(string expression)
        {
            return GetRowEvaluatedValue(CurrentControllerRow, expression);
        }

        public object GetRowEvaluatedValue(int rowIndex, string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return null;

            // Evaluate the expression against the current record row
            var evaluator = CreateExpressionEvaluator(expression);

            if (evaluator == null) return null;

            return evaluator.Evaluate(rowIndex);
        }
        #endregion

        #region IDataControllerData
        UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns()
        {
            UnboundColumnInfoCollection result = new();
            
            foreach(var property in calculatedFieldManagerService.CalculatedFieldProperties)
            {
                result.Add(new UnboundColumnInfo(property.Name, property.PropertyType, false, property.Expression, property.IsVisible));
            }
           
            return result;
        }

        object IDataControllerData.GetUnboundData(int listSourceRow1, DataColumnInfo column, object value)
        {
            return value;
        }

        void IDataControllerData.SetUnboundData(int listSourceRow1, DataColumnInfo column, object value)
        {

        }

        #endregion
    }
}
