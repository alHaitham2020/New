using ISoft.RichReports.Data.Fields;
using System;
using System.Collections.Concurrent;

namespace ISoft.RichReports.Services.Editor.Implementations
{
    /// <summary>
    /// Manage a dataSource data fields
    /// </summary>
    public class RichDataFieldsManagerService : IRichDataFieldsManagerService
    {
        private readonly IRichDataFieldsProvider _fieldsProvider;
        private readonly ConcurrentDictionary<string, RichFieldDataInfo> _fieldCache = new();

        public RichDataFieldsManagerService(IRichDataFieldsProvider fieldsProvider)
        {
            _fieldsProvider = fieldsProvider ?? throw new ArgumentNullException(nameof(fieldsProvider));
        }

        /// <summary>
        /// returns the field data name (in data source)
        /// </summary>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public string GeFieldActualDataName(string fieldName)
        {
            var dataInfo = GetFieldInfo(fieldName);
            if (dataInfo == null) return fieldName;

            // If the field is a detail field, return just the name without the parent table name (dataMember)
            if (dataInfo.IsDetailField)
            {
                return dataInfo.Name; // name without parent tablename (dataMember) for ex (AttachmentTable.Number) => (Number)
            }

            return fieldName;
        }

        /// <summary>
        /// Gets the display value for a field, handling null/empty checks and image formatting.
        /// </summary>
        public object GetFieldDisplayValue(string fieldName, object fieldValue)
        {
            var dataInfo = GetFieldInfo(fieldName);

            if (dataInfo == null) return fieldValue;

            // Return null for empty/null values based on field's data type and null value definition
            if (dataInfo.AllowUseDefaultEmptyValue && DataValidationHelper.IsEmptyValue(fieldValue, dataInfo.DataType, dataInfo.NullValue, false))
                return null;

            return GetDisplayValueCore(dataInfo, fieldValue);
        }

        /// <summary>
        /// Gets the display name for a field, falling back to empty string if not found.
        /// </summary>
        public string GetFieldDisplayName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return string.Empty;

            return GetFieldInfo(fieldName)?.DisplayName ?? string.Empty;
        }

        /// <summary>
        /// Retrieves field information with caching for performance.
        /// Note: ConcurrentDictionary's GetOrAdd is thread-safe, so explicit locking is unnecessary.
        /// </summary>
        public IRichFieldDataInfo GetFieldInfo(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return null;

            //if(_fieldCache.TryGetValue(fieldName, out var dataInfo))
            //{
            //    return dataInfo;
            //}

            //var dtField = _fieldsProvider.Fields.FindFieldByPath(fieldName);
            //if(dtField == null) return null;

            //dataInfo = CreateRichFieldInfo(dtField);
            //_fieldCache[fieldName] = dataInfo;

            // GetOrAdd handles thread-safety internally, no need for explicit lock
            return _fieldCache.GetOrAdd(fieldName, key =>
            {
                var dtField = _fieldsProvider.DataFields.FindFieldByPath(fieldName);
                return dtField != null ? CreateRichFieldInfo(dtField) : null;
            });
        }

        /// <summary>
        /// Core display value logic - handles image fields specially, otherwise uses default formatting.
        /// </summary>
        protected virtual object GetDisplayValueCore(IRichFieldDataInfo dataInfo, object fieldValue)
        {
            // Route image fields to specialized handler
            if (dataInfo.IsImageField)
                return HandleImageFieldValue(dataInfo.ImageSchemeType, fieldValue);

            // Default formatting for non-image fields
            return DisplayValueHelper.GetDisplayValue(fieldValue);
        }

        /// <summary>
        /// Handles conversion of image field values based on the URI scheme type.
        /// Uses pattern matching for cleaner type checking and conversion.
        /// </summary>
        protected virtual object HandleImageFieldValue(ImageUriSchemeType schemeType, object fieldValue)
        {
            return ImageUri.CreateFromSchemeType(schemeType, fieldValue);
        }

        /// <summary>
        /// Creates a lightweight DTO from the provider's field information.
        /// </summary>
        protected virtual RichFieldDataInfo CreateRichFieldInfo(IRichDataField dtField)
        {
            return new RichFieldDataInfo
            {
                Name = dtField.Name,
                DisplayName = dtField.DisplayName,
                Path = dtField.FieldProperty,
                DataType = dtField.DataType,
                NullValue = dtField.NullValue,
                AllowUseDefaultEmptyValue = dtField.AllowUseDefaultEmptyValue,
                IsDetailField = dtField.IsDetailField,
                IsImageField = dtField.IsImageField,
                ImageSchemeType = dtField.ImageSchemeType
            };
        }

        public void InvalidateCache()
        {
            _fieldCache.Clear();
        }
    }
}
