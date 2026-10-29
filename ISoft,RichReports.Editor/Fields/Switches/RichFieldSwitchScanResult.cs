namespace ISoft_RichReports.Editor.Fields.Switches
{
    public class RichFieldSwitchScanResult
    {
        public int StartIndex { get; set; }

        public string SwitchesString { get; set; }

        public bool Success { get; set; }

        public static RichFieldSwitchScanResult CreateEmpty()
        {
            return new RichFieldSwitchScanResult
            {
                StartIndex = -1,
                SwitchesString = null,
                Success = false
            };
        }
    }
}
