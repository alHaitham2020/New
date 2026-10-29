using System;
using System.Globalization;

namespace ISoft.RichReports.Editor.Fields.Formatting
{
    public readonly struct RichFieldFormat : IEquatable<RichFieldFormat>
    {
        #region Static
        public static readonly RichFieldFormat Empty = new(
            RichFieldFormatType.None,
            string.Empty,
            string.Empty,
            string.Empty);

        // Stable samples used for preview values
        private static readonly DateTime SampleDateTime = new(System.DateTime.Now.Year, 3, 15, 14, 30, 45, 123);

        private static readonly decimal SampleDecimal = 1234567.891m;

        #endregion

        #region Fields
        private readonly RichFieldFormatType _type;
        private readonly string _name;
        private readonly string _value;
        private readonly string _displayName;
        private readonly string _formatedValue;
        #endregion

        #region Proprties
        public RichFieldFormatType FormatType => _type;

        public string Name => _name ?? string.Empty;

        public string Value => _value ?? string.Empty;

        public string DisplayName => _displayName ?? string.Empty;

        public string FormatedValue => _formatedValue ?? string.Empty;

        public bool IsValid() =>
           _type != RichFieldFormatType.None &&
           !string.IsNullOrWhiteSpace(_name) &&
           !string.IsNullOrWhiteSpace(_value);

        #endregion

        public RichFieldFormat(
            RichFieldFormatType type,
            string name,
            string value,
            string displayName = null)
        {
            _type = type;
            _name = name ?? string.Empty;
            _value = value ?? string.Empty;
            _displayName = displayName ?? string.Empty;

            _formatedValue = BuildFormattedValue(type, value);

            //var dataToSign = Encoding.UTF8.GetBytes($"{response.LicenseId}{response.AppId}{response.CreatedDate:O}{response.ExpiryDate:O}");

        }

        #region Factories
        public static RichFieldFormat Numeric(
            string name,
            string value,
            string displayName = null) =>
            new(RichFieldFormatType.Numeric, name, value, displayName);

        public static RichFieldFormat DateTime(
            string name,
            string value,
            string displayName = null) =>
            new(RichFieldFormatType.DateTime, name, value, displayName);

        public static RichFieldFormat General(
            string name,
            string value,
            string displayName = null) =>
            new(RichFieldFormatType.General, name, value, displayName);
        #endregion

        #region Equality
        public bool Equals(RichFieldFormat other) =>
            _type == other._type &&
            string.Equals(_name, other._name, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(_value, other._value, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) =>
            obj is RichFieldFormat other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = new HashCode();
                hash.Add(_type);
                hash.Add(_name, StringComparer.OrdinalIgnoreCase);
                hash.Add(_value, StringComparer.OrdinalIgnoreCase);
                hash.Add(_displayName, StringComparer.OrdinalIgnoreCase);
                return hash.ToHashCode();
            }
        }

        public static bool operator ==(RichFieldFormat left, RichFieldFormat right) =>
            left.Equals(right);

        public static bool operator !=(RichFieldFormat left, RichFieldFormat right) =>
            !left.Equals(right);

        public override string ToString() => Value;

        /// <summary>
        /// Checks if a nullable format is null or empty.
        /// </summary>
        public static bool IsNullOrEmpty(RichFieldFormat? format) =>
            !format.HasValue || !format.Value.IsValid();
        #endregion

        #region Formatted-value builders

        private static string BuildFormattedValue(RichFieldFormatType type, string format)
        {
            if (string.IsNullOrWhiteSpace(format))
                return string.Empty;

            return type switch
            {
                RichFieldFormatType.DateTime => FormatDateTime(format),
                RichFieldFormatType.Numeric => FormatNumeric(format),
                //RichFieldFormatType.General => FormatGeneral(format),
                _ => string.Empty,
            };
        }

        private static string FormatDateTime(string format)
        {
            try
            {
                var displayFormat = GetDisplayFormat(format);
                return string.IsNullOrEmpty(displayFormat)
                    ? string.Empty
                    : string.Format(CultureInfo.CurrentCulture, displayFormat, SampleDateTime);
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        private static string FormatNumeric(string format)
        {
            try
            {
                var displayFormat = GetDisplayFormat(format);
                return string.IsNullOrEmpty(displayFormat)
                    ? string.Empty
                    : string.Format(CultureInfo.CurrentCulture, displayFormat, SampleDecimal);
            }
            catch (FormatException)
            {
                return string.Empty;
            }
        }

        //private static string FormatGeneral(string format)
        //{
        //    try
        //    {
        //        // For General, treat the value as a literal format string.
        //        // If it contains a placeholder, format it; otherwise return as-is.
        //        return format.Contains("{")
        //            ? string.Format(CultureInfo.CurrentCulture, format, SampleGeneral)
        //            : format;
        //    }
        //    catch (FormatException)
        //    {
        //        return string.Empty;
        //    }
        //}

        private static string GetDisplayFormat(string displayFormat)
        {
            if (string.IsNullOrEmpty(displayFormat))
                return string.Empty;

            if (displayFormat.Contains("{"))
                return displayFormat;

            return $"{{0:{displayFormat}}}";
        }
        #endregion
    }
}
