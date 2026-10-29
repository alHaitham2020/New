using ISoft.RichReports.Editor.Fields.Switches;
using System;

namespace ISoft.RichReports.Editor.Fields.Formatting
{
    /// <summary>
    /// Format definitions for common data types (please move it to the new above startegy)
    /// </summary>
    public static class RichFieldFormatHelper
    {
        public static string GetDefaultFormatValue(Type type)
        {
            return RichFieldFormatManager.Current.GetDefaultFormatValue(type);
        }

        public static RichFieldFormat? GetDefaultFormat(Type type)
        {
            return RichFieldFormatManager.Current.GetDefaultFormat(type);
        }

        public static RichFieldFormat? GetFormatOrDefault(this RichFieldFormat? format, Type type)
        {
            if (!RichFieldFormat.IsNullOrEmpty(format)) 
                return format;

            return GetDefaultFormat(type);
        }

        public static RichFieldFormat? GetDateTimeFormat(RichFieldDateTimeFormatType type)
        {
            return RichFieldFormatManager.Current.GetDateTimeFormatProvider().GetFormat(type);
        }

        public static RichFieldFormat? GetNumericFormat(RichFieldNumericFormatType type)
        {
            return RichFieldFormatManager.Current.GetNumericFormatProvider().GetFormat(type);
        }

        public static RichFieldFormat? GetGeneralFormat(RichFieldGeneralFormatType type)
        {
            return RichFieldFormatManager.Current.GetGeneralFormatProvider().GetFormat(type);
        }

        public static RichSwitchType ToSwitchType(this RichFieldFormatType formatType)
        {
            return formatType switch
            {
                RichFieldFormatType.None => RichSwitchType.None,
                RichFieldFormatType.Numeric => RichSwitchType.Numeric,
                RichFieldFormatType.DateTime => RichSwitchType.DateTime,
                RichFieldFormatType.General => RichSwitchType.General,
                _ => throw new NotImplementedException(nameof(formatType)),
            };
        }

        public static RichFieldFormatType ToFormatType(this RichSwitchType switchType)
        {
            return switchType switch
            {
                RichSwitchType.None => RichFieldFormatType.None,
                RichSwitchType.Numeric => RichFieldFormatType.Numeric,
                RichSwitchType.DateTime => RichFieldFormatType.DateTime,
                RichSwitchType.General => RichFieldFormatType.General,
                _ => throw new NotImplementedException(nameof(switchType)),
            };
        }
    }
}
