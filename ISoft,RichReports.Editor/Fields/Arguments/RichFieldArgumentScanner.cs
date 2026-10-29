using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ISoft.RichReports.Editor.Fields.Argumentes
{
    /// <summary>
    /// Specialized scanner for parsing quoted arguments in Word field codes.
    /// Handles extracting, replacing, and manipulating quoted string arguments.
    /// </summary>
    /// <remarks>
    /// Thread-safe implementation. Regex patterns are compiled once and reused.
    /// Supports both simple argument extraction and advanced manipulation.
    /// </remarks>
    public class RichFieldArgumentScanner : IRichFieldArgumentScanner
    {
        #region Constants
        private const string ARGUMENT_PATTERN = @"""([^""]*)""";
        private const int ARGUMENT_CONTENT_GROUP = 1;

        // More permissive pattern for edge cases (escaped quotes, etc.)
        private const string ARGUMENT_PERMISSIVE_PATTERN = @"""(?:[^""\\]|\\.)*""";
        #endregion

        #region Static Fields
        /// <summary>
        /// Compiled regex for extracting quoted arguments.
        /// </summary>
        private static readonly Regex ArgumentRegex = new Regex(ARGUMENT_PATTERN, RegexOptions.Compiled);

        /// <summary>
        /// Compiled regex for permissive argument extraction.
        /// </summary>
        private static readonly Regex ArgumentPermissiveRegex = new Regex(ARGUMENT_PERMISSIVE_PATTERN, RegexOptions.Compiled);
        #endregion

        #region Core Extraction Methods

        /// <summary>
        /// Extracts all quoted arguments from a rawText string.
        /// </summary>
        /// <param name="rawText">String containing quoted arguments</param>
        /// <param name="permissiveMode">If true, handles escaped quotes and edge cases</param>
        /// <returns>List of argument values (without quotes)</returns>
        public List<string> ExtractArguments(string rawText, bool permissiveMode = false)
        {
            var arguments = new List<string>();

            if (string.IsNullOrWhiteSpace(rawText))
                return arguments;

            var regex = permissiveMode ? ArgumentPermissiveRegex : ArgumentRegex;
            var matches = regex.Matches(rawText);

            foreach (Match match in matches)
            {
                string value;
                if (permissiveMode)
                {
                    // Remove outer quotes and unescape inner quotes
                    value = match.Value.Trim('"').Replace("\\\"", "\"");
                }
                else
                {
                    value = match.Groups[ARGUMENT_CONTENT_GROUP].Value;
                }

                arguments.Add(value);
            }

            return arguments;
        }

        /// <summary>
        /// Extracts a specific argument by index.
        /// </summary>
        /// <param name="rawText">String containing quoted arguments</param>
        /// <param name="index">Zero-based index of the argument</param>
        /// <param name="permissiveMode">If true, handles escaped quotes and edge cases</param>
        /// <returns>Argument value or null if not found</returns>
        public string ExtractArgument(string rawText, int index, bool permissiveMode = false)
        {
            if (string.IsNullOrWhiteSpace(rawText) || index < 0)
                return null;

            var arguments = ExtractArguments(rawText, permissiveMode);
            return index < arguments.Count ? arguments[index] : null;
        }

        /// <summary>
        /// Extracts the first argument from a rawText string.
        /// </summary>
        public string ExtractFirstArgument(string rawText, bool permissiveMode = false)
        {
            return ExtractArgument(rawText, 0, permissiveMode);
        }

        /// <summary>
        /// Extracts arguments and their positions in the original string.
        /// </summary>
        public List<ArgumentPosition> ExtractArgumentsWithPositions(string rawText)
        {
            var result = new List<ArgumentPosition>();

            if (string.IsNullOrWhiteSpace(rawText))
                return result;

            var matches = ArgumentRegex.Matches(rawText);
            for (int i = 0; i < matches.Count; i++)
            {
                var match = matches[i];

                if (match.Groups[ARGUMENT_CONTENT_GROUP].Success)
                {
                    result.Add(new ArgumentPosition
                    {
                        Index = i,
                        Value = match.Groups[ARGUMENT_CONTENT_GROUP].Value,
                        StartIndex = match.Index,
                        Length = match.Length,
                        EndIndex = match.Index + match.Length
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Counts the number of arguments in a rawText string.
        /// </summary>
        public int CountArguments(string rawText, bool permissiveMode = false)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return 0;

            var regex = permissiveMode ? ArgumentPermissiveRegex : ArgumentRegex;
            return regex.Matches(rawText).Count;
        }

        #endregion

        #region Manipulation Methods

        /// <summary>
        /// Removes all quoted arguments from a rawText string.
        /// </summary>
        public string RemoveArguments(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
                return rawText;

            return ArgumentRegex.Replace(rawText, string.Empty).Trim();
        }

        /// <summary>
        /// Removes a specific argument by index.
        /// </summary>
        public string RemoveArgument(string rawText, int index)
        {
            if (string.IsNullOrWhiteSpace(rawText) || index < 0)
                return rawText;

            var args = ExtractArguments(rawText);
            if (index >= args.Count)
                return rawText;

            // Build pattern to match the specific argument
            // This is a simplified approach - for production, use more robust parsing
            var escapedArg = Regex.Escape(args[index]);
            var pattern = $@"""{escapedArg}""";
            var regex = new Regex(pattern);

            // Find and remove the nth occurrence
            var matches = regex.Matches(rawText);
            if (matches.Count > index)
            {
                var match = matches[index];
                return rawText.Remove(match.Index, match.Length).Trim();
            }

            return rawText;
        }

        /// <summary>
        /// Replaces the first quoted argument with a new value.
        /// </summary>
        public string ReplaceFirstArgument(string rawText, string newValue)
        {
            if (string.IsNullOrWhiteSpace(rawText) || newValue == null)
                return rawText;

            return ArgumentRegex.Replace(rawText, $"\"{newValue}\"", 1);
        }

        /// <summary>
        /// Replaces a specific argument by index with a new value.
        /// </summary>
        public string ReplaceArgument(string rawText, int index, string newValue)
        {
            if (string.IsNullOrWhiteSpace(rawText) || index < 0 || newValue == null)
                return rawText;

            var args = ExtractArguments(rawText);
            if (index >= args.Count)
                return rawText;

            // Build pattern to match the specific argument
            var escapedArg = Regex.Escape(args[index]);
            var pattern = $@"""{escapedArg}""";
            var regex = new Regex(pattern);

            // Find and replace the nth occurrence
            var matches = regex.Matches(rawText);
            if (matches.Count > index)
            {
                var match = matches[index];
                return rawText
                    .Remove(match.Index, match.Length)
                    .Insert(match.Index, $"\"{newValue}\"");
            }

            return rawText;
        }

        /// <summary>
        /// Adds a new argument to the rawText string.
        /// </summary>
        public string AppendArgument(string rawText, string newValue, bool appendAtEnd = true)
        {
            if (newValue == null)
                return rawText;

            var quotedArg = $"\"{newValue}\"";
            if (string.IsNullOrWhiteSpace(rawText))
                return quotedArg;

            return appendAtEnd
                ? $"{rawText} {quotedArg}".Trim()
                : $"{quotedArg} {rawText}".Trim();
        }

        #endregion

        #region Validation Methods

        /// <summary>
        /// Validates if a string is a properly quoted argument.
        /// </summary>
        public bool IsValidArgument(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return false;

            // Must start and end with quotes, and not contain unescaped quotes inside
            return text.StartsWith("\"") &&
                   text.EndsWith("\"") &&
                   !Regex.IsMatch(text.Substring(1, text.Length - 2), @"(?<!\\)""");
        }

        /// <summary>
        /// Formats a value as a quoted argument.
        /// </summary>
        public string FormatArgument(string value)
        {
            if (value == null)
                return " \"\"";

            // Escape any inner quotes
            var escaped = value.Replace("\"", "\\\"");
            return $" \"{escaped}\"";
        }
        #endregion
    }
}
