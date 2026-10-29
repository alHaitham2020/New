using System.Collections.Generic;

namespace ISoft_RichReports.Editor.Fields.Switches
{
    public interface IRichFieldSwitchScanner
    {
        List<string> ExtractSwitchCodes(string rawCodeText, bool includeBackslash = true);

        IRichFieldSwitch FindSpecificSwitch(string rawCodeText, string targetSwitchCode);

        RichFieldSwitchInfo GetSwitchInfo(string rawCodeText, string switchCode);

        bool IsValidSwitchCode(string switchCode);

        string RemoveSpecificSwitch(string rawCodeText, string switchCode);

        string RemoveSwitches(string rawCodeText);

        void ScanSwitches(IRichFieldSwitchCollection switches, string rawCodeText);

        IRichFieldSwitchReadOnlyCollection ScanSwitches(string rawCodeText);

        bool TryScanSwitches(IRichFieldSwitchCollection switches, string rawText, out int switchStartIndex);
    }
}
