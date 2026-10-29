using ISoft_RichReports.Editor.Fields.Arguments;
using ISoft_RichReports.Editor.Fields.Core;
using ISoft_RichReports.Editor.Fields.Switches;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ISoft_RichReports.Editor.Fields.Codes
{
    /// <summary>
    /// Parses field codes from document text into structured RichFieldCode objects.
    /// Handles Microsoft Word field codes including MERGEFIELD, INCLUDEPICTURE, IF, HYPERLINK, etc.
    /// </summary>
    /// <remarks>
    /// Thread-safe implementation. All regex patterns are compiled once and reused.
    /// Uses specialized scanners for switches and arguments for better separation of concerns.
    /// </remarks>
    public class RichFieldCodeScanner : IRichFieldCodeScanner
    {
        #region Singletone
        private static readonly Lazy<IRichFieldCodeScanner> _instance = new(() => new RichFieldCodeScanner());

        public static IRichFieldCodeScanner Default => _instance.Value;
        #endregion

        #region Constants
        // Pattern groups: Group 1 = field type, Group 2 = remaining parameters
        private const int FIELD_TYPE_GROUP = 1;
        private const int PARAMETERS_GROUP = 2;

        // Field code pattern - optimized with non-capturing groups where possible
        private const string FIELD_TYPE_PATTERN_TEMPLATE = @"^({0})\s*(.*)";
        #endregion

        #region Static Fields
        /// <summary>
        /// Primary regex for extracting field type and parameters from field codes.
        /// </summary>
        private static readonly Regex FieldCodeRegex;

        /// <summary>
        /// Specialized scanner for switches.
        /// </summary>
        private readonly IRichFieldSwitchScanner _switchScanner;

        /// <summary>
        /// Specialized scanner for arguments.
        /// </summary>
        private readonly IRichFieldArgumentScanner _argumentScanner;

        #endregion

        #region Constructor
        /// <summary>
        /// Static constructor to initialize compiled regex patterns and supported types cache.
        /// </summary>
        static RichFieldCodeScanner()
        {
            // Build field code pattern
            var fieldTypes = RichEditFieldInfoProvider.GetAllFieldsName();
            var pattern = string.Format(
                FIELD_TYPE_PATTERN_TEMPLATE,
                string.Join("|", fieldTypes.Select(Regex.Escape)));

            FieldCodeRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        public RichFieldCodeScanner(IRichFieldSwitchScanner switchScanner = null, IRichFieldArgumentScanner argumentScanner = null)
        {
            // Initialize specialized scanners
            _switchScanner = switchScanner ?? new RichFieldSwitchScanner();
            _argumentScanner = argumentScanner ?? new RichFieldArgumentScanner();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Parses a single field code string into a structured RichFieldCode object.
        /// </summary>
        public IRichFieldCode Scan(string fieldCode)
        {
            if (string.IsNullOrWhiteSpace(fieldCode))
                return null;

            fieldCode = fieldCode.Trim();

            var match = FieldCodeRegex.Match(fieldCode);
            return ScanCore(match, fieldCode);
        }

        /// <summary>
        /// Parses multiple field codes from a text block.
        /// </summary>
        public IRichFieldCodeCollection ScanMultiple(string text)
        {
            var fields = new RichFieldCodeCollection();

            if (string.IsNullOrWhiteSpace(text))
                return fields;

            // Pre-allocate capacity based on initial match count
            var matches = FieldCodeRegex.Matches(text);
            fields.Capacity = matches.Count;

            foreach (Match match in matches)
            {
                var fieldCode = ScanCore(match);
                if (fieldCode != null)
                {
                    fields.Add(fieldCode);
                }
            }

            return fields;
        }

        #endregion

        #region Core Scanning

        private RichFieldCode ScanCore(Match match, string originalCode = null)
        {
            var code = originalCode ?? (match.Success ? match.Value : string.Empty);

            if (!match.Success)
            {
                return new RichFieldCode(RichEditFieldInfo.Unknown)
                {
                    OriginalCode = code
                };
            }

            var fieldTypeString = match.Groups[FIELD_TYPE_GROUP].Value.Trim();
            var parameters = match.Groups[PARAMETERS_GROUP].Value.Trim();

            var fieldCode = new RichFieldCode(fieldTypeString)
            {
                OriginalCode = code
            };

            // Parse parameters using specialized scanners
            ParseParameters(fieldCode, parameters);

            return fieldCode;
        }

        #endregion

        #region Parameter Parsing

        private void ParseParameters(RichFieldCode fieldCode, string parameters)
        {
            if (string.IsNullOrWhiteSpace(parameters))
                return;

            // Parse switches ( its important to parse switches first then arguments)
            _switchScanner.ScanSwitches(fieldCode.Switches, parameters);

            var cleanedParameters = _switchScanner.RemoveSwitches(parameters);

            // Use argument scanner to extract quoted arguments
            var arguments = _argumentScanner.ExtractArgumentsWithPositions(cleanedParameters);

            // Add arguments
            foreach (var arg in arguments)
            {
                fieldCode.Arguments.AddOrUpdateValue(arg.Index, arg.Value);
            }
        }

        #endregion
     
        #region Utility Methods
        public IRichFieldSwitchReadOnlyCollection ScanSwitches(string rawCodeText)
        {
            return _switchScanner.ScanSwitches(rawCodeText);
        }

        public IRichFieldSwitch FindSpecificSwitch(string rawCodeText, string targetSwitchCode)
        {
            return _switchScanner.FindSpecificSwitch(rawCodeText, targetSwitchCode);
        }

        public RichFieldSwitchInfo GetSwitchInfo(string rawCodeText, string switchCode)
        {
            return _switchScanner.GetSwitchInfo(rawCodeText, switchCode);
        }

        #endregion

        //#region Specialized Field Type Parsers

        ///// <summary>
        ///// Parses INCLUDEPICTURE field with arguments and dimension switches.
        ///// Handles both static image paths and MERGEFIELD-based dynamic paths.
        ///// </summary>
        ///// <param name="fieldCode">Raw INCLUDEPICTURE field code</param>
        ///// <returns>Structured options object or null if parsing fails</returns>
        ///// <remarks>
        ///// Example: INCLUDEPICTURE "path" "QRCode" \w 100 \h 100
        ///// - Arguments: ["path", "QRCode"]
        ///// - Switches: \w 100, \h 100
        ///// Supports barcode integration via ImageOption parameter.
        ///// </remarks>
        //public IncludePictureFieldOptions ParseIncludePictureCode(string fieldCode)
        //{
        //    if (string.IsNullOrWhiteSpace(fieldCode))
        //        return null;

        //    // Use general scanner to parse field structure
        //    var scanned = Scan(fieldCode);

        //    // Validate: must be IncludePicture type
        //    if (scanned == null || scanned.FieldType != RichFieldType.IncludePictureField)
        //        return null;

        //    var options = new IncludePictureFieldOptions
        //    {
        //        // First argument is always the image path
        //        ImagePath = scanned.Arguments.GetFirstValue(),

        //        // Second argument may contain barcode type or other options
        //        ImageOption = scanned.Arguments.GetStringValue(1)
        //    };

        //    // Extract width from \w switch
        //    var widthSwitch = scanned.Switches.FirstOrDefault(s => s.SwitchCode == "\\w");
        //    if (widthSwitch != null && int.TryParse(widthSwitch.SwitchValue, out int width))
        //    {
        //        options.Width = width;
        //    }

        //    // Extract height from \h switch
        //    var heightSwitch = scanned.Switches.FirstOrDefault(s => s.SwitchCode == "\\h");
        //    if (heightSwitch != null && int.TryParse(heightSwitch.SwitchValue, out int height))
        //    {
        //        options.Height = height;
        //    }

        //    // Check for MERGEFORMAT switch (preserves formatting on field update)
        //    options.PreserveFormatting = scanned.Switches.Any(s =>
        //        s.SwitchCode == "\\*" &&
        //        s.SwitchValue?.IndexOf("MERGEFORMAT", StringComparison.OrdinalIgnoreCase) >= 0);

        //    // Check for \d switch (prevents image embedding, stores link only)
        //    options.LinkOnly = scanned.Switches.Any(s =>
        //        s.SwitchCode?.Equals("\\d", StringComparison.OrdinalIgnoreCase) == true);

        //    // Detect MERGEFIELD usage in path for dynamic image resolution
        //    if (options.ImagePath != null)
        //    {
        //        options.HasMergeField = options.ImagePath.IndexOf("MERGEFIELD", StringComparison.OrdinalIgnoreCase) >= 0 ||
        //                               options.ImagePath.Contains('«'); // Check for field placeholder characters

        //        if (options.HasMergeField)
        //        {
        //            options.MergeFieldName = ExtractMergeFieldName(options.ImagePath);
        //        }
        //    }

        //    return options;
        //}

        ///// <summary>
        ///// Parses IF field conditional statement.
        ///// Extracts expression, true text, and false text from field code.
        ///// </summary>
        ///// <param name="fieldCode">Raw IF field code</param>
        ///// <returns>Structured options or null if parsing fails</returns>
        ///// <remarks>
        ///// Example: IF "condition" "true text" "false text"
        ///// - Property: condition expression
        ///// - Argument 0: text to display if true
        ///// - Argument 1: text to display if false
        ///// </remarks>
        //public IfFieldOptions ParseIfFieldCode(string fieldCode)
        //{
        //    var scanned = Scan(fieldCode) as RichFieldCode;

        //    if (scanned == null || scanned.FieldType != RichFieldType.IfField)
        //        return null;

        //    return new IfFieldOptions
        //    {
        //        Expression = scanned.Property,      // Conditional expression
        //        TrueText = scanned.Arguments.GetStringValue(0),  // Display when condition is true
        //        FalseText = scanned.Arguments.GetStringValue(1)  // Display when condition is false
        //    };
        //}

        ///// <summary>
        ///// Parses HYPERLINK field with URL and display text.
        ///// </summary>
        ///// <param name="fieldCode">Raw HYPERLINK field code</param>
        ///// <returns>Structured options or null if parsing fails</returns>
        ///// <remarks>
        ///// Example: HYPERLINK "https://url.com" "Click Here"
        ///// - Argument 0: URL target
        ///// - Argument 1: Display text (optional)
        ///// </remarks>
        //public HyperlinkFieldOptions ParseHyperlinkFieldCode(string fieldCode)
        //{
        //    var scanned = Scan(fieldCode) as RichFieldCode;

        //    if (scanned == null || scanned.FieldType != RichFieldType.HyperlinkField)
        //        return null;

        //    return new HyperlinkFieldOptions
        //    {
        //        Url = scanned.Arguments.GetFirstValue(),    // Link target URL
        //        DisplayText = scanned.Arguments.GetStringValue(1) // Optional display text
        //    };
        //}

        ///// <summary>
        ///// Parses DOCVARIABLE field with variable name and default value.
        ///// Handles both property-based and argument-based variable naming conventions.
        ///// </summary>
        ///// <param name="fieldCode">Raw DOCVARIABLE field code</param>
        ///// <returns>Structured options or null if parsing fails</returns>
        ///// <remarks>
        ///// Flexible parsing: variable name can be in property or first argument position.
        ///// Example 1: DOCVARIABLE "varName" "defaultValue"
        ///// Example 2: DOCVARIABLE varName "defaultValue"
        ///// </remarks>
        //public DocVariableFieldOptions ParseDocVariableFieldCode(string fieldCode)
        //{
        //    var scanned = Scan(fieldCode) as RichFieldCode;

        //    if (scanned == null || scanned.FieldType != RichFieldType.DocVariableField)
        //        return null;

        //    return new DocVariableFieldOptions
        //    {
        //        // Variable name: prefer property, fall back to first argument
        //        VariableName = scanned.Property ?? scanned.Arguments.GetFirstValue(),

        //        // Default value: position depends on where variable name was found
        //        DefaultValue = scanned.Arguments.GetStringValue(scanned.Property != null ? 0 : 1)
        //    };
        //}

        ///// <summary>
        ///// Parses custom BARCODE field with barcode type, value, and dimensions.
        ///// Typically implemented as a specialized MERGEFIELD with additional arguments.
        ///// </summary>
        ///// <param name="fieldCode">Raw barcode field code</param>
        ///// <returns>Structured options or null if parsing fails</returns>
        ///// <remarks>
        ///// Example: MERGEFIELD Barcode "QRCode" "ProductCode" 100 50
        ///// - Argument 0: Barcode type (QRCode, Code128, etc.)
        ///// - Argument 1: Value to encode
        ///// - Argument 2/3: Width/Height (optional numeric arguments)
        ///// </remarks>
        //public BarcodeFieldOptions ParseBarcodeFieldCode(string fieldCode)
        //{
        //    var scanned = Scan(fieldCode);

        //    if (scanned == null)
        //        return null;

        //    var options = new BarcodeFieldOptions
        //    {
        //        // Value to encode: from argument 1 or property as fallback
        //        Value = scanned.Arguments.GetStringValue(1) ?? scanned.Property,

        //        // Dimensions: parsed as integers from arguments 2 and 3
        //        Width = scanned.Arguments.GetValue<int>(2),

        //        Height = scanned.Arguments.GetValue<int>(3),

        //        // Parse barcode type from first argument
        //        BarcodeType = scanned.Arguments.GetStringValue(0).TryConvert(FieldBarcodeType.None)
        //    };

        //    return options;
        //}

        //public string ExtractMergeFieldName(string path)
        //{
        //    if (string.IsNullOrWhiteSpace(path))
        //        return null;

        //    // Strategy 1: Explicit MERGEFIELD keyword
        //    var mergeMatch = Regex.Match(
        //        path,
        //        @"MERGEFIELD\s+(\w+)",
        //        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        //    if (mergeMatch.Success)
        //    {
        //        return mergeMatch.Groups[1].Value;
        //    }

        //    // Strategy 2: Word field placeholder characters «FieldName»
        //    var placeholderMatch = Regex.Match(path, @"«([^»]+)»");
        //    if (placeholderMatch.Success)
        //    {
        //        return placeholderMatch.Groups[1].Value;
        //    }

        //    return null;
        //}

        //public string ReplaceImagePath(string fieldCode, string newPath)
        //{
        //    if (string.IsNullOrWhiteSpace(fieldCode) || string.IsNullOrWhiteSpace(newPath))
        //        return fieldCode;

        //    return _argumentScanner.ReplaceFirstArgument(fieldCode, newPath);
        //}

        //public string ReplacePathWithMergeField(string fieldCode, string mergeFieldName)
        //{
        //    if (string.IsNullOrWhiteSpace(fieldCode) || string.IsNullOrWhiteSpace(mergeFieldName))
        //        return fieldCode;

        //    return _argumentScanner.ReplaceFirstArgument(fieldCode, $"«{mergeFieldName}»");
        //}

        //public string UpdateImageDimensions(string fieldCode, int? width, int? height)
        //{
        //    if (string.IsNullOrWhiteSpace(fieldCode))
        //        return fieldCode;

        //    // Remove existing dimension switches
        //    fieldCode = _switchScanner.RemoveSpecificSwitch(fieldCode, "\\w");
        //    fieldCode = _switchScanner.RemoveSpecificSwitch(fieldCode, "\\h");

        //    // Append new dimensions
        //    if (width.HasValue)
        //        fieldCode += $" \\w {width.Value}";

        //    if (height.HasValue)
        //        fieldCode += $" \\h {height.Value}";

        //    // Clean up spacing
        //    return Regex.Replace(fieldCode.Trim(), @"\s+", " ");
        //}

        //public string UpdateImageHeight(string fieldCode, int? height)
        //{
        //    if (string.IsNullOrWhiteSpace(fieldCode))
        //        return fieldCode;

        //    // Remove existing dimension switches
        //    fieldCode = _switchScanner.RemoveSpecificSwitch(fieldCode, "\\h");

        //    if (height.HasValue)
        //        fieldCode += $" \\h {height.Value}";

        //    // Clean up spacing
        //    return Regex.Replace(fieldCode.Trim(), @"\s+", " ");
        //}

        //#endregion

    }
}
