using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Internal.Fields;
using ISoft.RichReports.Editor.Services.Abstractions;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Editor.Services.Implementations
{
    /// <summary>
    /// Provides comprehensive information about all rich edit fields with localization support
    /// </summary>
    public class RichEditFieldsManagerService : IRichEditFieldsManagerService
    {
        private readonly IRichEditFieldDisplayNameProvider _displayNameProvider;

        public RichEditFieldsManagerService() : this(new RichEditFieldDisplayNameProvider())
        {

        }

        public RichEditFieldsManagerService(IRichEditFieldDisplayNameProvider displayNameProvider)
        {
            _displayNameProvider = displayNameProvider;
        }

        #region RichEditFieldsManagerService Implemntation
        public string GetFieldDisplayName(RichEditFieldType fieldType)
        {
            return _displayNameProvider.GetDisplayName(fieldType);
        }

        /// <summary>
        /// Gets field information by field type
        /// </summary>
        /// <param name="fieldType">The field type</param>
        /// <returns>The field information</returns>
        public RichEditFieldInfo? GetFieldInfo(RichEditFieldType fieldType)
        {
            return RichEditFieldInfoProvider.GetFieldInfoByType(fieldType);
        }

        /// <summary>
        /// Tries to get field information by field type
        /// </summary>
        public bool TryGetFieldInfo(RichEditFieldType fieldType, out RichEditFieldInfo info)
        {
            return RichEditFieldInfoProvider.TryGetFieldInfoByType(fieldType, out info);
        }

        /// <summary>
        /// Gets field information by field name (case-insensitive)
        /// </summary>
        /// <param name="fieldName">The field name</param>
        /// <returns>The field information if found</returns>
        public RichEditFieldInfo? GetFieldInfoByName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                throw new ArgumentException("Field name cannot be null or empty", nameof(fieldName));

            return RichEditFieldInfoProvider.GetFieldInfoByName(fieldName);
        }

        /// <summary>
        /// Tries to get field information by field name
        /// </summary>
        public bool TryGetFieldInfoByName(string fieldName, out RichEditFieldInfo info)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                info = default;
                return false;
            }

            return RichEditFieldInfoProvider.TryGetFieldInfoByName(fieldName, out info);

        }

        /// <summary>
        /// Gets all field information
        /// </summary>
        /// <returns>Dictionary of all field information</returns>
        public IReadOnlyDictionary<RichEditFieldType, RichEditFieldInfo> GetAllFieldInfos()
        {
            return RichEditFieldInfoProvider.GetAllFields().ToFrozenDictionary(x => x.FieldType);
        }

        /// <summary>
        /// Gets all field names
        /// </summary>
        public IEnumerable<string> GetAllFieldNames()
        {
            return RichEditFieldInfoProvider.GetAllFieldsName();
        }

        /// <summary>
        /// Gets fields of a specific type
        /// </summary>
        /// <param name="type">The field value type</param>
        /// <returns>Fields that return the specified type</returns>
        public IEnumerable<RichEditFieldInfo> GetFieldsByType(Type type)
        {
            ArgumentGuardHelper.ArgumentNotNull(type);
            return RichEditFieldInfoProvider.GetAllFields().Where(info => info.Type == type);
        }

        /// <summary>
        /// Checks if a field type exists
        /// </summary>
        public bool HasFieldType(RichEditFieldType fieldType)
        {
            return RichEditFieldInfoProvider.IsSupportedFieldType(fieldType);
        }

        /// <summary>
        /// Checks if a field name exists
        /// </summary>
        public bool HasFieldName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return false;

            return RichEditFieldInfoProvider.IsSupportedFieldType(fieldName);
        }

        /// <summary>
        /// Gets the field type by field name
        /// </summary>
        public RichEditFieldType? GetFieldTypeByName(string fieldName)
        {
            if (TryGetFieldInfoByName(fieldName, out var info))
                return info.FieldType;

            return null;
        }
        #endregion
    }
}
