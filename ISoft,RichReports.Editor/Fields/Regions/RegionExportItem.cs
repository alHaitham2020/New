using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    public class RegionExportItem
    {
        public string Name { get; set; }

        public string ParentName { get; set; }

        public int MergeFieldCount { get; set; }

        public List<string> MergeFieldNames { get; set; } = new List<string>();

        public int NestedRegionCount { get; set; }

        public int StartPosition { get; set; }

        public int EndPosition { get; set; }

        public int Depth { get; set; }

        public bool IsValid { get; set; }

        public List<string> ValidationMessages { get; set; } = new List<string>();
    }
}
