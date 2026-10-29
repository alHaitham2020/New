using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
using System;
using ISoft.RichReports.Editor.Data;
using ISoft.RichReports.Editor.Internal.Fields;

namespace ISoft.RichReports.Editor.Services.Implementation
{
    //public class DynamicObjectPropertyValueProvider : IDynamicObjectPropertyValueProvider
    //{
    //    public object GetPropertyValue(object instance, string propertyName)
    //    {
    //        return instance.GetPropertyValueFromPath(propertyName);
    //    }

    //    public bool IsDynamicTypeInstance(object instance)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class RichEditMailMergeDataService : MailMergeDataService
    {
        #region Fields
        private readonly IRichDataFieldsManagerService _dataManagerService;
        private readonly IRichEditCalculatedFieldManagerService _calculatedFieldManagerService;

        internal RichDataControllerAdapter Adapter => DataController as RichDataControllerAdapter;
        #endregion

        #region Constructors
        internal RichEditMailMergeDataService(
            IRichDataFieldsManagerService dataManagerService,
            IRichEditCalculatedFieldManagerService calculatedFieldManagerService)
            : this(dataManagerService, calculatedFieldManagerService,
                   RichDataControllerAdapter.CreateDefault(calculatedFieldManagerService))
        {

        }

        public RichEditMailMergeDataService(
            IRichDataFieldsManagerService dataManagerService,
            IRichEditCalculatedFieldManagerService calculatedFieldManagerService,
            RichEditDataControllerAdapterBase dataController)
            : this(dataManagerService, calculatedFieldManagerService, dataController, 0, -1)
        {

        }

        public RichEditMailMergeDataService(
            IRichDataFieldsManagerService dataManagerService,
            IRichEditCalculatedFieldManagerService calculatedFieldManagerService,
            RichEditDataControllerAdapterBase dataController,
            int firstRecordIndex,
            int lastRecordIndex)
            : base(dataController, firstRecordIndex, lastRecordIndex)
        {
            _dataManagerService = dataManagerService;
            _calculatedFieldManagerService = calculatedFieldManagerService;
        }
        #endregion

        #region Overrides
        protected override MailMergeDataService CreateNew(
            RichEditDataControllerAdapterBase dataController,
            IMailMergeOptions options)
        {
            return new RichEditMailMergeDataService(
                _dataManagerService,
                _calculatedFieldManagerService,
                dataController,
                options.FirstRecordIndex,
                options.LastRecordIndex);
        }

        protected override RichEditDataControllerAdapterBase CreateMailMergeDataController()
        {
            return RichDataControllerAdapter.CreateDefault(_calculatedFieldManagerService);
        }

        public override object GetFieldValue(
            MailMergeProperties mailMergeProperties,
            string fieldName,
            bool mapFieldName,
            MailMergeDataMode options,
            PieceTable pieceTable,
            Field field)
        {
            // Calculated fields are resolved by the base implementation
            if (field.CachedFieldType == RichExpressionEditorCalculatedField.FieldType)
                return base.GetFieldValue(mailMergeProperties, fieldName, mapFieldName, options, pieceTable, field);

            var actualFieldName = GetActualDataFieldName(fieldName);
            var fieldValue = base.GetFieldValue(mailMergeProperties, actualFieldName, mapFieldName, options, pieceTable, field);

            return ResolveDataFieldDisplayValue(fieldName, fieldValue);
        }
        #endregion

        #region Helpers

        protected virtual string GetActualDataFieldName(string fieldName)
        {
            return _dataManagerService.GeFieldActualDataName(fieldName);
        }

        protected object GetFieldValueFromDataSource(string fieldName)
        {
            var actualDataName = _dataManagerService.GeFieldActualDataName(fieldName);
            var fieldValue = Adapter.GetFieldValueFromDataSource(actualDataName);

            return ResolveDataFieldDisplayValue(fieldName, fieldValue);
        }

        //protected RichMergeField GetMergeField(Field field)
        //{
        //    return new RichEditDocumentModelFieldParser().GetMergeField(field);
        //}

        private object ResolveDataFieldDisplayValue(string fieldName, object fieldValue)
        {
            if (fieldValue == null || fieldValue == DBNull.Value)
                return null;

            return _dataManagerService.GetFieldDisplayValue(fieldName, fieldValue);
        }
        #endregion
    }
}
