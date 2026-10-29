namespace ISoft.RichReports.Editor.Fields.Regions
{
    public class RegionValidationWarning
    {
        public string RegionName { get; set; }

        public string Message { get; set; }

        public WarningType Type { get; set; }

        public enum WarningType
        {
            NoMergeFields,
            EmptyNestedRegions,
            PotentialUnclosedRegion,
            DuplicateName
        }
    }
}
