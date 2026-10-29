namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Search mode for finding regions in a range
    /// </summary>
    public enum RegionSearchMode
    {
        /// <summary>Region overlaps with the target range</summary>
        Overlapping,

        /// <summary>Region is completely inside the target range</summary>
        FullyContained,

        /// <summary>Region completely contains the target range</summary>
        Containing
    }
}
