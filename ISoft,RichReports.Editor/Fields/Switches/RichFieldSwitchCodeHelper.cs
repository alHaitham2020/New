using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public static class RichFieldSwitchCodeHelper
    {
        public const string Separator = "\\";

        // Cache for field type strings
        private static readonly Dictionary<string, RichSwitchType> _standardSwitchTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { RichStandardFieldSwitchKeys.GeneralKey, RichSwitchType.General },
            { RichStandardFieldSwitchKeys.DateTimeKey, RichSwitchType.DateTime },
            { RichStandardFieldSwitchKeys.NumericKey, RichSwitchType.Numeric },
        };

        #region Key
        /// <summary>
        /// Determines the format type of a switch
        /// </summary>
        public static RichSwitchType GetSwitchType(string switchKey)
        {
            if (string.IsNullOrEmpty(switchKey)) return RichSwitchType.None;

            switchKey = switchKey.Trim();

            // Try exact match first (fastest)
            if (_standardSwitchTypes.TryGetValue(switchKey, out var fieldType))
                return fieldType;

            return RichSwitchType.FieldSpecific;
        }

        /// <summary>
        /// Normalizes a switch code to ensure it starts with a backslash.
        /// </summary>
        /// <param name="switchKey">The switch code to normalize</param>
        /// <returns>Switch code with leading backslash, or null if input is null/empty</returns>
        /// <example>
        /// NormalizeSwitchCode("w") -> "\w"
        /// NormalizeSwitchCode("\w") -> "\w"
        /// NormalizeSwitchCode("@") -> "\@"
        /// </example>
        public static string NormalizeSwitchCode(string switchKey)
        {
            if (string.IsNullOrWhiteSpace(switchKey))
                return switchKey;

            switchKey = switchKey.Trim();

            // If it already starts with backslash, return as-is
            if (switchKey.StartsWith(Separator))
                return switchKey;

            // Add backslash prefix
            return Separator + switchKey;
        }

        /// <summary>
        /// Normalizes a switch code to ensure it does NOT start with a backslash.
        /// </summary>
        /// <param name="switchKey">The switch code to normalize</param>
        /// <returns>Switch code without leading backslash, or null if input is null/empty</returns>
        /// <example>
        /// DenormalizeSwitchCode("\w") -> "w"
        /// DenormalizeSwitchCode("w") -> "w"
        /// DenormalizeSwitchCode("\@") -> "@"
        /// </example>
        public static string DenormalizeSwitchCode(string switchKey)
        {
            if (string.IsNullOrWhiteSpace(switchKey))
                return switchKey;

            switchKey = switchKey.Trim();

            // Remove leading backslash if present
            if (switchKey.StartsWith(Separator))
                return switchKey.Substring(1);

            return switchKey;
        }

        /// <summary>
        /// Normalizes a switch code with explicit control over backslash inclusion.
        /// </summary>
        /// <param name="switchKey">The switch code to normalize</param>
        /// <param name="includeBackslash">If true, ensures backslash is present; if false, ensures it's removed</param>
        /// <returns>Normalized switch code</returns>
        /// <example>
        /// NormalizeSwitchCode("w", true) -> "\w"
        /// NormalizeSwitchCode("\w", true) -> "\w"
        /// NormalizeSwitchCode("\w", false) -> "w"
        /// NormalizeSwitchCode("w", false) -> "w"
        /// </example>
        public static string NormalizeSwitchCode(string switchKey, bool includeBackslash)
        {
            if (string.IsNullOrWhiteSpace(switchKey))
                return switchKey;

            return includeBackslash ? NormalizeSwitchCode(switchKey) : DenormalizeSwitchCode(switchKey);
        }

        /// <summary>
        /// Determines if a switch code has a leading backslash.
        /// </summary>
        /// <param name="switchKey">The switch code to check</param>
        /// <returns>True if the switch code starts with a backslash, false otherwise</returns>
        public static bool HasLeadingBackslash(string switchKey)
        {
            if (string.IsNullOrWhiteSpace(switchKey))
                return false;

            return switchKey.Trim().StartsWith(Separator);
        }

        /// <summary>
        /// Extracts the core switch character (without backslash) from a switch code.
        /// </summary>
        /// <param name="switchKey">The switch code (with or without backslash)</param>
        /// <returns>The core switch character, or null if input is invalid</returns>
        /// <example>
        /// GetCoreSwitchCode("\w") -> "w"
        /// GetCoreSwitchCode("w") -> "w"
        /// GetCoreSwitchCode("\@") -> "@"
        /// </example>
        public static string GetCleanSwitchCode(string switchKey)
        {
            if (string.IsNullOrWhiteSpace(switchKey))
                return switchKey;

            var trimmed = switchKey.Trim();
            return trimmed.StartsWith(Separator) ? trimmed.Substring(1) : trimmed;
        }

        /// <summary>
        /// Validates if a switch code contains only valid switch characters.
        /// </summary>
        /// <param name="switchKey">The switch code to validate</param>
        /// <returns>True if the switch code contains only valid characters, false otherwise</returns>
        public static bool IsValidSwitchCharacter(string switchKey)
        {
            if (string.IsNullOrWhiteSpace(switchKey))
                return false;

            var core = GetCleanSwitchCode(switchKey);
            if (string.IsNullOrEmpty(core))
                return false;

            // Valid switch characters are @, #, *, and letters (a-z, A-Z)
            return Regex.IsMatch(core, @"^[@#\*a-zA-Z]+$");
        }
        #endregion

        #region Value
        /// <summary>
        /// Normalizes a switch value to ensure it is wrapped in double quotes.
        /// </summary>
        /// <param name="switchValue">The switch value to normalize</param>
        /// <returns>Switch value wrapped in double quotes, or null/empty if input is null/empty</returns>
        /// <example>
        /// NormalizeSwitchValue("hello") -> "\"hello\""
        /// NormalizeSwitchValue("\"hello\"") -> "\"hello\""
        /// NormalizeSwitchValue("") -> ""
        /// NormalizeSwitchValue(null) -> null
        /// </example>
        public static string NormalizeSwitchValue(string switchValue)
        {
            if (string.IsNullOrWhiteSpace(switchValue))
                return switchValue;

            switchValue = switchValue.Trim();

            // If it already starts and ends with quotes, return as-is
            if (switchValue.StartsWith("\"") && switchValue.EndsWith("\"") && switchValue.Length >= 2)
                return switchValue;

            // Remove any existing quotes and re-wrap
            var core = DenormalizeSwitchValue(switchValue);
            return $"\"{core}\"";
        }

        /// <summary>
        /// Normalizes a switch value to ensure it does NOT have surrounding double quotes.
        /// </summary>
        /// <param name="switchValue">The switch value to normalize</param>
        /// <returns>Switch value without surrounding double quotes, or null/empty if input is null/empty</returns>
        /// <example>
        /// DenormalizeSwitchValue("\"hello\"") -> "hello"
        /// DenormalizeSwitchValue("hello") -> "hello"
        /// DenormalizeSwitchValue("\"\"") -> ""
        /// DenormalizeSwitchValue(null) -> null
        /// </example>
        public static string DenormalizeSwitchValue(string switchValue)
        {
            if (string.IsNullOrWhiteSpace(switchValue))
                return switchValue;

            switchValue = switchValue.Trim();

            // Remove surrounding quotes if present
            if (switchValue.StartsWith("\"") && switchValue.EndsWith("\"") && switchValue.Length >= 2)
                return switchValue.Substring(1, switchValue.Length - 2);

            return switchValue;
        }

        /// <summary>
        /// Normalizes a switch value with explicit control over quote inclusion.
        /// </summary>
        /// <param name="switchValue">The switch value to normalize</param>
        /// <param name="includeQuotes">If true, ensures quotes are present; if false, ensures they're removed</param>
        /// <returns>Normalized switch value</returns>
        /// <example>
        /// NormalizeSwitchValue("hello", true) -> "\"hello\""
        /// NormalizeSwitchValue("\"hello\"", true) -> "\"hello\""
        /// NormalizeSwitchValue("\"hello\"", false) -> "hello"
        /// NormalizeSwitchValue("hello", false) -> "hello"
        /// </example>
        public static string NormalizeSwitchValue(string switchValue, bool includeQuotes)
        {
            if (string.IsNullOrWhiteSpace(switchValue))
                return switchValue;

            return includeQuotes ? NormalizeSwitchValue(switchValue) : DenormalizeSwitchValue(switchValue);
        }

        /// <summary>
        /// Determines if a switch value has surrounding double quotes.
        /// </summary>
        /// <param name="switchValue">The switch value to check</param>
        /// <returns>True if the switch value is wrapped in double quotes, false otherwise</returns>
        public static bool HasQuotes(string switchValue)
        {
            if (string.IsNullOrWhiteSpace(switchValue))
                return false;

            var trimmed = switchValue.Trim();
            return trimmed.StartsWith("\"") && trimmed.EndsWith("\"") && trimmed.Length >= 2;
        }

        /// <summary>
        /// Extracts the core switch value (without quotes) from a switch value.
        /// </summary>
        /// <param name="switchValue">The switch value (with or without quotes)</param>
        /// <returns>The core switch value without quotes, or null if input is invalid</returns>
        /// <example>
        /// GetCleanSwitchValue("\"hello\"") -> "hello"
        /// GetCleanSwitchValue("hello") -> "hello"
        /// </example>
        public static string GetCleanSwitchValue(string switchValue)
        {
            return DenormalizeSwitchValue(switchValue);
        }

        #endregion

        #region Code
        /// <summary>
        /// Builds the switch string
        /// </summary>
        public static string Build(string switchKey, string switchValue)
        {
            var code = NormalizeSwitchCode(switchKey);

            if (string.IsNullOrEmpty(switchValue)) // value may be null because some fields have only switchKey without value
                return $" {code}"; // keep space before

            var normalizedValue = NormalizeSwitchValue(switchValue);
            return $" {code} {normalizedValue}";
        }
        #endregion
    }
}
