using ISoft.RichReports.Editor.Fields.Switches;

namespace ISoft.RichReports.Editor.Fields.Codes
{
    public interface IRichFieldCodeScanner
    {
        IRichFieldCode Scan(string rawCodeText);

        IRichFieldCodeCollection ScanMultiple(string rawCodeText);

        IRichFieldSwitchReadOnlyCollection ScanSwitches(string parameters);

        IRichFieldSwitch FindSpecificSwitch(string parameters, string targetSwitchKey);

        RichFieldSwitchInfo GetSwitchInfo(string rawCodeText, string switchKey);
    }
}
