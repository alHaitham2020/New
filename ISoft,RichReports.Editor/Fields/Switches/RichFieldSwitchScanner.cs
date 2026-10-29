using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    /// <summary>
    /// Specialized scanner for parsing Word field switches.
    /// Handles switches like \@ "format", \# "format", \* MERGEFORMAT, \w 100, etc.
    /// </summary>
    /// <remarks>
    /// Thread-safe implementation. All regex patterns are compiled once and reused.
    /// Supports both standalone switch parsing and extraction from parameter strings.
    /// </remarks>
    public class RichFieldSwitchScanner : IRichFieldSwitchScanner
    {
        #region Constants
        private const string SWITCH_PATTERN = @"(\\[@#\*a-zA-Z]+)(?:\s+(""[^""]*""|[^\\]+))?";
        public const string SWITCH_START_PATTERN = @"\\([@#\*a-zA-Z])";

        private const int SWITCH_CODE_GROUP = 1;
        private const int SWITCH_VALUE_GROUP = 2;
        #endregion

        #region Static Fields
        /// <summary>
        /// Compiled regex for parsing switches.
        /// </summary>
        private static readonly Regex SwitchRegex = new(SWITCH_PATTERN, RegexOptions.Compiled);

        /// <summary>
        /// Regex for finding switch start positions.
        /// </summary>
        private static readonly Regex SwitchStartRegex = new(SWITCH_START_PATTERN, RegexOptions.Compiled);
        #endregion

        #region Core Scanning Methods
        public IRichFieldSwitch ScanSwitch(string switcheStr)
        {
            if (string.IsNullOrWhiteSpace(switcheStr))
                return null;

            switcheStr = switcheStr.Trim();

            // Extract switch portion and parse
            var matche = SwitchRegex.Match(switcheStr);

            return ScanSwitchCore(matche);
        }

        /// <summary>
        /// Parses all switches from a parameters string.
        /// </summary>
        /// <param name="rawText">String containing switch definitions</param>
        /// <param name="includeBackslash">If true, switch codes include leading backslash; if false, backslash is removed</param>
        /// <returns>Collection of parsed switches</returns>
        public IRichFieldSwitchReadOnlyCollection ScanSwitches(string rawText)
        {
            var switches = new RichFieldSwitchCollection();
            ScanSwitches(switches, rawText);
            return switches;
        }

        /// <summary>
        /// Parses all switches from a parameters string into an existing collection.
        /// </summary>
        /// <param name="switches">Collection to populate</param>
        /// <param name="rawText">String containing switch definitions</param>
        /// <param name="includeBackslash">If true, switch codes include leading backslash; if false, backslash is removed</param>
        public void ScanSwitches(IRichFieldSwitchCollection switches, string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText) || switches == null)
                return;

            rawText = rawText.Trim();

            var scanResult = GetSwitchScanResult(rawText);
            if (!scanResult.Success)
            {
                return;
            }

            // Use switch scanner
            ScanSwitchesFromClean(switches, scanResult.SwitchesString);
        }

        public bool TryScanSwitches(IRichFieldSwitchCollection switches, string rawText, out int switchStartIndex)
        {
            if (string.IsNullOrWhiteSpace(rawText) || switches == null)
            {
                switchStartIndex = -1;
                return false;
            }

            var scanResult = GetSwitchScanResult(rawText);
            if (!scanResult.Success)
            {
                switchStartIndex = -1;
                return false;
            }

            ScanSwitchesFromClean(switches, scanResult.SwitchesString);

            switchStartIndex = scanResult.StartIndex;
            return true;
        }

        /// <summary>
        /// Finds and parses a specific switch from a parameters string.
        /// </summary>
        /// <param name="rawText">String containing switch definitions</param>
        /// <param name="targetSwitchCode">Switch code to find (e.g., "\w", "\@")</param>
        /// <param name="includeBackslash">If true, switch codes include leading backslash; if false, backslash is removed</param>
        /// <returns>Parsed switch or null if not found</returns>
        public IRichFieldSwitch FindSpecificSwitch(string rawText, string targetSwitchCode)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return null;

            // Denormalize the target switch code to match the expected format
            var code = RichFieldSwitchCodeHelper.DenormalizeSwitchCode(targetSwitchCode);

            return ScanSwitches(rawText).GetItemByKey(code);
        }

        /// <summary>
        /// Extracts all switch codes from a parameters string.
        /// </summary>
        /// <param name="rawText">String containing switch definitions</param>
        /// <param name="includeBackslash">If true, returned codes include leading backslash; if false, backslash is removed</param>
        /// <returns>List of switch codes</returns>
        public List<string> ExtractSwitchCodes(string rawText, bool includeBackslash = true)
        {
            var codes = new List<string>();

            if (string.IsNullOrWhiteSpace(rawText))
                return codes;

            var matches = SwitchStartRegex.Matches(rawText);
            foreach (Match match in matches)
            {
                var code = match.Value;
                if (!includeBackslash)
                {
                    code = code.RemoveFirst(); // Remove leading backslash
                }
                codes.Add(code);
            }

            return codes;
        }
       
        protected Match GetSwitchStartMatch(string rawText)
        {
            return SwitchStartRegex.Match(rawText);
        }

        protected RichFieldSwitchScanResult GetSwitchScanResult(string rawText)
        {
            var result = RichFieldSwitchScanResult.CreateEmpty();

            if (string.IsNullOrWhiteSpace(rawText))
            {
                return result;
            }

            // Find the first switch marker
            var switchStart = GetSwitchStartMatch(rawText);

            if (switchStart.Success)
            {
                // Extract switches portion
                var switchesStr = rawText.Substring(switchStart.Index);

                result.StartIndex = switchStart.Index;
                result.SwitchesString = switchesStr;
                result.Success = true;
            }

            return result;
        }

        /// <summary>
        /// Parses all switches from a parameters string into an existing collection.
        /// </summary>
        /// <param name="switches">Collection to populate</param>
        /// <param name="switchesStr">String containing switch definitions</param>
        /// <param name="includeBackslash">If true, switch codes include leading backslash; if false, backslash is removed</param>
        protected void ScanSwitchesFromClean(IRichFieldSwitchCollection switches, string switchesStr)
        {
            if (string.IsNullOrWhiteSpace(switchesStr) || switches == null)
                return;

            // Extract switch portion and parse
            var matches = SwitchRegex.Matches(switchesStr);

            foreach (Match match in matches)
            {
                var sw = ScanSwitchCore(match);
                if (sw != null)
                {
                    switches.AddOrUpdate(sw);
                }
            }
        }
        #endregion

        #region Core Parsing

        /// <summary>
        /// Core switch parsing logic shared across methods.
        /// </summary>
        private RichFieldSwitch ScanSwitchCore(Match match)
        {
            if (!match.Success)
                return null;
            
            var switchCode = match.Groups[SWITCH_CODE_GROUP].Success
                ? match.Groups[SWITCH_CODE_GROUP].Value
                : null;

            //Remove leading backslash
            if (switchCode != null)
            {
                switchCode = RichFieldSwitchCodeHelper.DenormalizeSwitchCode(switchCode);
            }

            var switchValue = match.Groups[SWITCH_VALUE_GROUP].Success
                 ? match.Groups[SWITCH_VALUE_GROUP].Value.Trim().Trim('"') // remove the queted
                 : null;


            var switchType = switchCode != null
                ? RichFieldSwitchCodeHelper.GetSwitchType(switchCode)
                : RichSwitchType.None;

            return new RichFieldSwitch(switchType, switchCode, switchValue);
        }

        #endregion

        #region Utility Methods
        public RichFieldSwitchInfo GetSwitchInfo(string rawText, string switchCode)
        {
            if (string.IsNullOrEmpty(rawText)) return RichFieldSwitchInfo.Empty;

            // Normalize the switch code for pattern matching
            var normalizedCode = RichFieldSwitchCodeHelper.NormalizeSwitchCode(switchCode);
            string pattern = $@"(?<code>{Regex.Escape(normalizedCode)})\s*(?<val>\d+|""[^""]+"")";
            Match match = Regex.Match(rawText, pattern);

            if (!match.Success) return RichFieldSwitchInfo.Empty;

            // Get the code group
            Group codeGroup = match.Groups["code"];
            int codeStartIndex = codeGroup.Success ? codeGroup.Index : -1;
            int codeLength = codeGroup.Success ? codeGroup.Length : -1;

            // Get the value group
            Group valueGroup = match.Groups["val"];
            int valueStartIndex = valueGroup.Success ? valueGroup.Index : -1;
            int valueLength = valueGroup.Success ? valueGroup.Length : -1;
            string value = valueGroup.Success ? valueGroup.Value : string.Empty;

            var codeInfo = new RichFieldSwitchPartInfo(codeStartIndex, codeLength);
            var valueInfo = new RichFieldSwitchPartInfo(valueStartIndex, valueLength);

            return new RichFieldSwitchInfo(switchCode, value, codeInfo, valueInfo);
        }

        /// <summary>
        /// Validates if a switch code is in the correct format.
        /// </summary>
        public bool IsValidSwitchCode(string switchCode)
        {
            if (string.IsNullOrWhiteSpace(switchCode))
                return false;

            return SwitchStartRegex.IsMatch(switchCode);
        }

        /// <summary>
        /// Removes all switches from a parameters string.
        /// </summary>
        public string RemoveSwitches(string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
                return parameters;

            var switchMatch = SwitchStartRegex.Match(parameters);
            if (!switchMatch.Success)
                return parameters;

            // Extract text before the first switch
            return parameters.Substring(0, switchMatch.Index).Trim();
        }

        /// <summary>
        /// Removes a specific switch from a parameters string.
        /// </summary>
        public string RemoveSpecificSwitch(string parameters, string switchCode)
        {
            if (string.IsNullOrWhiteSpace(parameters))
                return null;

            parameters = parameters.Trim();
            var pattern = GetSwitchPattern(switchCode);
            if (string.IsNullOrWhiteSpace(pattern))
                return parameters;

            return Regex.Replace(parameters, pattern, "").Trim();
        }

        protected string GetSwitchPattern(string targetSwitchCode)
        {
            if (string.IsNullOrWhiteSpace(targetSwitchCode))
                return null;

            var escapedSwitchCode = Regex.Escape(targetSwitchCode);
            return $@"({escapedSwitchCode})(\s+[^\\]*)?(?=\\|$)";
        }

        #endregion
    }
}
