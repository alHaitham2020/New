using System;

namespace ISoft_RichReports.Editor.Fields.Switches
{
    /// <summary>
    /// represent info for switch key /value
    /// </summary>
    public readonly struct RichFieldSwitchPartInfo : IEquatable<RichFieldSwitchPartInfo>
    {
        public static readonly RichFieldSwitchPartInfo Empty = new(-1, -1);

        private readonly int _index;
        private readonly int _length;

        public int StartIndex => _index;

        public int Length => _length;

        public bool IsEmpty => this == Empty;

        public RichFieldSwitchPartInfo(int index, int length)
        {
            _index = index;
            _length = length;
        }

        public override bool Equals(object obj) => obj is RichFieldSwitchPartInfo other && Equals(other);

        public bool Equals(RichFieldSwitchPartInfo other) =>
            StartIndex == other.StartIndex && Length == other.Length;

        #region Operators
        public static bool operator ==(RichFieldSwitchPartInfo left, RichFieldSwitchPartInfo right) => left.Equals(right);
        public static bool operator !=(RichFieldSwitchPartInfo left, RichFieldSwitchPartInfo right) => !left.Equals(right);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion
    }

}
