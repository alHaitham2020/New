using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Export data for regions
    /// </summary>
    public class RegionExportData
    {
        public DateTime ExportDate { get; set; } = DateTime.Now;

        public List<RegionExportItem> Regions { get; set; } = new List<RegionExportItem>();

        public RegionStatistics Statistics { get; set; }
    }
}
