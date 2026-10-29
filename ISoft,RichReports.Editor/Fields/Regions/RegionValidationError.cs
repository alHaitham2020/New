namespace ISoft.RichReports.Editor.Fields.Regions
{
    public class RegionValidationError
    {
        public string RegionName { get; set; }

        public string Message { get; set; }

        public ErrorType Type { get; set; }

        public enum ErrorType
        {
            MissingStartField,
            MissingEndField,
            InvalidName,
            EmptyRegion,
            MalformedStructure
        }
    }
}
