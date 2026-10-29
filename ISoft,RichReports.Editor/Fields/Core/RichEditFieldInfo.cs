using ISoft.RichReports.Editor.Fields.Formatting;
using System;

namespace ISoft.RichReports.Editor.Fields.Core
{
    public readonly struct RichEditFieldInfo : IEquatable<RichEditFieldInfo>
    {
        public static readonly RichEditFieldInfo Unknown = new(RichEditFieldType.Unknown, string.Empty, null, RichEditFieldCategory.None, false);

        private readonly RichEditFieldType _fieldType;
        private readonly RichEditFieldCategory _category;
        private readonly string _fieldName;
        private readonly Type _type;
        private readonly bool _supportProperty;

        public RichEditFieldCategory Category => _category;

        public RichEditFieldType FieldType => _fieldType;

        public string FieldName => _fieldName;

        /// <summary>
        /// the calculated value type of field
        /// </summary>
        public Type Type => _type;

        /// <summary>
        /// if field has a fieldProperty (first argument)
        /// </summary>
        public bool SupportProperty => _supportProperty;

        /// <summary>
        /// Default Format
        /// </summary>
        public RichFieldFormat? DefaultFormat => RichFieldFormatHelper.GetDefaultFormat(_type);

        public bool IsValid() =>
            _fieldType != RichEditFieldType.Unknown &&
            !string.IsNullOrWhiteSpace(_fieldName);

        public RichEditFieldInfo(
            RichEditFieldType fieldType,
            string fieldName,
            Type type,
            RichEditFieldCategory category,
            bool? supportProperty = null)
        {
            _fieldType = fieldType;
            _fieldName = fieldName ?? string.Empty;
            _type = type;
            _category = category;
            _supportProperty = supportProperty ?? false;
        }

        #region Factory methods
        public static RichEditFieldInfo StringField(RichEditFieldType fieldType, string fieldName, RichEditFieldCategory category, bool? supportProperty = null) =>
            new(fieldType, fieldName, typeof(string), category, supportProperty);

        public static RichEditFieldInfo DateTimeField(RichEditFieldType fieldType, string fieldName, RichEditFieldCategory category, bool? supportProperty = null) =>
            new(fieldType, fieldName, typeof(DateTime), category, supportProperty);

        public static RichEditFieldInfo DateOnlyField(RichEditFieldType fieldType, string fieldName, RichEditFieldCategory category, bool? supportProperty = null) =>
            new(fieldType, fieldName, typeof(DateOnly), category, supportProperty);

        public static RichEditFieldInfo TimeOnlyField(RichEditFieldType fieldType, string fieldName, RichEditFieldCategory category, bool? supportProperty = null) =>
            new(fieldType, fieldName, typeof(TimeOnly), category, supportProperty);

        public static RichEditFieldInfo IntField(RichEditFieldType fieldType, string fieldName, RichEditFieldCategory category, bool? supportProperty = null) =>
            new(fieldType, fieldName, typeof(int), category, supportProperty);

        public static RichEditFieldInfo BoolField(RichEditFieldType fieldType, string fieldName, RichEditFieldCategory category, bool? supportProperty = null) =>
            new(fieldType, fieldName, typeof(bool), category, supportProperty);
        #endregion

        /// <summary>
        /// Checks if a nullable RichFieldInfo is null or invalid.
        /// </summary>
        public static bool IsNullOrEmpty(RichEditFieldInfo? field) =>
            !field.HasValue || !field.Value.IsValid();

        #region IEquatable<RichFieldInfo> implementation
        public bool Equals(RichEditFieldInfo other) =>
            _fieldType == other._fieldType &&
            _category == other._category &&
            string.Equals(_fieldName, other._fieldName, StringComparison.OrdinalIgnoreCase) &&
            _type == other._type &&
            _supportProperty == other._supportProperty;

        public override bool Equals(object obj) =>
            obj is RichEditFieldInfo other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + _fieldType.GetHashCode();
                hash = hash * 31 + _category.GetHashCode();
                hash = hash * 31 + (_fieldName?.GetHashCode() ?? 0);
                hash = hash * 31 + (_type?.GetHashCode() ?? 0);
                hash = hash * 31 + (_supportProperty.GetHashCode());
                return hash;
            }
        }

        public static bool operator ==(RichEditFieldInfo left, RichEditFieldInfo right) => left.Equals(right);

        public static bool operator !=(RichEditFieldInfo left, RichEditFieldInfo right) => !left.Equals(right);

        public override string ToString() => _fieldName;

        #endregion
    }
}
