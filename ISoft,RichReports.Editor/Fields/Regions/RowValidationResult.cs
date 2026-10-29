namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Validation result for a single row
    /// </summary>
    public class RowValidationResult
    {
        public int RowIndex { get; set; }

        public bool HasRegion { get; set; }

        public bool IsValid { get; set; }

        public string Message { get; set; }

        public int RegionCount { get; set; }
    }
}
