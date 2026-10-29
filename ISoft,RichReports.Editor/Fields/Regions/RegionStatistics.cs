using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Statistics about mail merge regions
    /// </summary>
    public class RegionStatistics
    {
        public int TotalRegions { get; set; }

        public int TotalMergeFields { get; set; }

        public int TotalNestedRegions { get; set; }

        public int MaxNestingDepth { get; set; }

        public List<string> RegionNames { get; set; } = new List<string>();

        public Dictionary<string, int> MergeFieldsPerRegion { get; set; } = new Dictionary<string, int>();
    }
}
