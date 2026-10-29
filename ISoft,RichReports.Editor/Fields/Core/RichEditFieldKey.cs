using System;

namespace ISoft.RichReports.Editor.Fields.Core
{
    public readonly struct RichEditFieldKey : IEquatable<RichEditFieldKey>
    {
        private readonly RichEditFieldInfo _fieldInfo;
        private readonly string _fieldProperty;

        public RichEditFieldInfo FieldInfo => _fieldInfo;

        public string FieldProperty => _fieldProperty;

        public RichEditFieldKey(RichEditFieldInfo fieldInfo, string fieldProperty = null)
        {
            _fieldInfo = fieldInfo;
            _fieldProperty = fieldProperty ?? string.Empty;
        }

        public static RichEditFieldKey CreateFromInfo(RichEditFieldInfo fieldInfo, string fieldProperty)
        {
            return new RichEditFieldKey(fieldInfo, fieldProperty);
        }

        public static RichEditFieldKey CreateFromType(RichEditFieldType fieldType, string fieldProperty)
        {
            return CreateFromInfo(RichEditFieldInfoProvider.GetRequiredFieldInfoByType(fieldType), fieldProperty);
        }

        public bool IsValid()
        {
            return _fieldInfo.IsValid() && (!_fieldInfo.SupportProperty || !string.IsNullOrEmpty(_fieldProperty));
        }

        public bool Equals(RichEditFieldKey other)
            => _fieldInfo == other._fieldInfo
            && (!_fieldInfo.SupportProperty || string.Equals(_fieldProperty, other._fieldProperty, StringComparison.OrdinalIgnoreCase));

        public override bool Equals(object obj)
            => obj is RichEditFieldKey other && Equals(other);

        public override int GetHashCode()
            => HashCode.Combine(_fieldInfo, _fieldProperty);
        
        public static bool operator ==(RichEditFieldKey left, RichEditFieldKey right)
            => left.Equals(right);

        public static bool operator !=(RichEditFieldKey left, RichEditFieldKey right)
            => !left.Equals(right);

        public override string ToString()
            => $"{_fieldInfo.FieldName}:{_fieldProperty}";

        public static bool IsNullOrEmpty(RichEditFieldKey? fieldKey)
            => !fieldKey.HasValue || !fieldKey.Value.IsValid();
    }

}
