using System;

namespace ISoft_RichReports.Editor.Fields.Switches
{
    public readonly struct RichFieldSwitchInfo : IEquatable<RichFieldSwitchInfo>
    {
        public static readonly RichFieldSwitchInfo Empty = new(string.Empty, string.Empty, RichFieldSwitchPartInfo.Empty, RichFieldSwitchPartInfo.Empty);

        private readonly string _key;
        private readonly string _value;

        private readonly RichFieldSwitchPartInfo _codeInfo;
        private readonly RichFieldSwitchPartInfo _valueInfo;

        public string Key => _key;

        public string Value => _value;

        public RichFieldSwitchPartInfo CodeInfo => _codeInfo;

        public RichFieldSwitchPartInfo ValueInfo => _valueInfo;

        public int Length => CodeInfo.Length + ValueInfo.Length;

        public bool IsEmpty => this == Empty;

        public bool IsValid => !IsEmpty && !CodeInfo.IsEmpty && !ValueInfo.IsEmpty;

        public RichFieldSwitchInfo(string key, string value, RichFieldSwitchPartInfo codeInfo, RichFieldSwitchPartInfo valueInfo)
        {
            _key = key;
            _value = value;
            _codeInfo = codeInfo;
            _valueInfo = valueInfo;
        }

        public override bool Equals(object obj) => obj is RichFieldSwitchInfo other && Equals(other);

        public bool Equals(RichFieldSwitchInfo other) =>
            string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase) && CodeInfo == other.CodeInfo && ValueInfo == other.ValueInfo;

        #region Operators
        public static bool operator ==(RichFieldSwitchInfo left, RichFieldSwitchInfo right) => left.Equals(right);
        public static bool operator !=(RichFieldSwitchInfo left, RichFieldSwitchInfo right) => !left.Equals(right);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

}
