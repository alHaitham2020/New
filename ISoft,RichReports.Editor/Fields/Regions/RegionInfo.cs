using ISoft.Utils.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Detailed information about a specific region
    /// </summary>
    public class RegionInfo : ObjectBase, IEquatable<RegionInfo>
    {
        public string Name { get; set; }

        public bool HasStartField { get; set; }

        public bool HasEndField { get; set; }

        public int StartPosition { get; set; }

        public int StartLength { get; set; }

        public int EndPosition { get; set; }

        public int EndLength { get; set; }

        public int MergeFieldCount { get; set; }

        public List<string> MergeFieldNames { get; set; } = new List<string>();

        public bool IsValid => HasStartField && HasEndField;

        public override object Clone()
        {
            var clone = (RegionInfo)base.Clone();
            clone.MergeFieldNames = new(MergeFieldNames);
            return clone;
        }

        public bool Equals(RegionInfo other)
        {
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) &&
                   HasStartField == other.HasStartField &&
                   HasEndField == other.HasEndField &&
                   StartPosition == other.StartPosition &&
                   EndPosition == other.EndPosition &&
                   MergeFieldCount == other.MergeFieldCount &&
                   MergeFieldNames.SequenceEqual(other.MergeFieldNames);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as RegionInfo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
