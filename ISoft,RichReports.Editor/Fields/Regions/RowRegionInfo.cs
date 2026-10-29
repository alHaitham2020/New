namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Detailed information about regions in a table row
    /// </summary>
    public class RowRegionInfo
    {
        public bool HasRegion { get; set; }

        public RegionInfoCollection RegionsInRow { get; set; } = new RegionInfoCollection();

        public bool IsValid { get; set; }

        public string ValidationMessage { get; set; }
    }
}
