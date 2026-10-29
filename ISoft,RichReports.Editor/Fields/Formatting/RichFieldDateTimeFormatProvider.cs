using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Formatting
{
    public enum RichFieldDateTimeFormatType
    {
        // Date-only formats
        ShortDate,
        MediumDate,
        LongDate,
        FullDate,
        MonthYear,
        MonthDay,        // renamed from DayMonth
        DayMonth,
        YearMonth,
        DayOfWeek,       // e.g., "Monday"
        ShortDayOfWeek,  // e.g., "Mon"

        // Time-only formats
        ShortTime,
        MediumTime,
        LongTime,
        FullTime,
        HourMinute24,      // 24-hour "HH:mm"
        HourMinute12,    // 12-hour "hh:mm tt"

        // Combined date + time
        DateTime,        // General "G"
        DateTimeShort,   // "g"
        DateTimeLong,    // "F"
        DateTimeFull,    // "U"
        //RoundTrip,       // "o"
    }

    public interface IRichFieldDateTimeFormatProvider : IRichFieldFormatProvider<RichFieldDateTimeFormatType>
    {

    }

    public class RichFieldDateTimeFormatProvider : RichFieldFormatProviderBase<RichFieldDateTimeFormatType>, IRichFieldDateTimeFormatProvider
    {
        public override RichFieldFormatType FormatType => RichFieldFormatType.DateTime;

        public RichFieldDateTimeFormatProvider()
        {

        }

        protected override void PopulateFormats()
        {
            // ── Date-only ──────────────────────────────────────────────
            Register(RichFieldDateTimeFormatType.ShortDate,
                RichFieldFormat.DateTime(
                    "ShortDate",
                    "dd-MM-yyyy",
                    "Short date format"));

            Register(RichFieldDateTimeFormatType.MediumDate,
                RichFieldFormat.DateTime(
                    "MediumDate",
                    "dd-MMM-yyyy",
                    "Medium date format"));

            Register(RichFieldDateTimeFormatType.LongDate,
                RichFieldFormat.DateTime(
                    "LongDate",
                    "dddd, dd MMMM yyyy",
                    "Long date format"));

            Register(RichFieldDateTimeFormatType.FullDate,
                RichFieldFormat.DateTime(
                    "FullDate",
                    "dddd, dd MMMM yyyy",
                    "Full date format"));

            Register(RichFieldDateTimeFormatType.MonthYear,
                RichFieldFormat.DateTime(
                    "MonthYear",
                    "MMMM yyyy",
                    "Month and year format"));

            Register(RichFieldDateTimeFormatType.DayMonth,
                RichFieldFormat.DateTime(
                    "DayMonth",
                    "dd MMMM",
                    "Day and month format"));

            Register(RichFieldDateTimeFormatType.MonthDay,
                RichFieldFormat.DateTime(
                    "MonthDay",
                    "MMMM dd",
                    "Month and day format"));

            Register(RichFieldDateTimeFormatType.YearMonth,
                RichFieldFormat.DateTime(
                    "YearMonth",
                    "yyyy-MM",
                    "Year and month format"));

            Register(RichFieldDateTimeFormatType.DayOfWeek,
                RichFieldFormat.DateTime(
                    "DayOfWeek",
                    "dddd",
                    "Full day name"));

            Register(RichFieldDateTimeFormatType.ShortDayOfWeek,
                RichFieldFormat.DateTime(
                    "ShortDayOfWeek",
                    "ddd",
                    "Abbreviated day name"));

            // ── Time-only ──────────────────────────────────────────────
            Register(RichFieldDateTimeFormatType.ShortTime,
                RichFieldFormat.DateTime(
                    "ShortTime",
                    "hh:mm tt",
                    "Short time format"));

            Register(RichFieldDateTimeFormatType.MediumTime,
                RichFieldFormat.DateTime(
                    "MediumTime",
                    "hh:mm tt",
                    "Medium time format"));

            Register(RichFieldDateTimeFormatType.LongTime,
                RichFieldFormat.DateTime(
                    "LongTime",
                    "hh:mm:ss tt",
                    "Long time format"));

            Register(RichFieldDateTimeFormatType.FullTime,
                RichFieldFormat.DateTime(
                    "FullTime",
                    "hh:mm:ss.fff tt",
                    "Full time with milliseconds"));

            Register(RichFieldDateTimeFormatType.HourMinute24,
                RichFieldFormat.DateTime(
                    "HourMinute24",
                    "HH:mm",
                    "24-hour hour and minute"));

            Register(RichFieldDateTimeFormatType.HourMinute12,
                RichFieldFormat.DateTime(
                    "HourMinute12",
                    "hh:mm",
                    "12-hour hour and minute"));

            // ── Combined date + time ───────────────────────────────────
            Register(RichFieldDateTimeFormatType.DateTime,
                RichFieldFormat.DateTime(
                    "DateTime",
                    "dd-MM-yyyy hh:mm:ss tt",
                    "General date and time"));

            Register(RichFieldDateTimeFormatType.DateTimeShort,
                RichFieldFormat.DateTime(
                    "DateTimeShort",
                    "dd-MM-yyyy hh:mm tt",
                    "Short date and time"));

            Register(RichFieldDateTimeFormatType.DateTimeLong,
                RichFieldFormat.DateTime(
                    "DateTimeLong",
                    "dddd, dd MMMM yyyy hh:mm:ss",
                    "Long date and time"));

            Register(RichFieldDateTimeFormatType.DateTimeFull,
                RichFieldFormat.DateTime(
                    "DateTimeFull",
                    "dddd, dd MMMM yyyy hh:mm:ss tt",
                    "Full universal date and time"));
        }

        protected override void PopulateDefaultFormats()
        {
            // Register default formats for types
            RegisterDefaultFormat(typeof(DateTime), RichFieldDateTimeFormatType.ShortDate);
            RegisterDefaultFormat(typeof(DateOnly), RichFieldDateTimeFormatType.ShortDate);
            RegisterDefaultFormat(typeof(TimeOnly), RichFieldDateTimeFormatType.ShortTime);
        }

        protected override List<RichFieldFormat> GetAvailableFormatsInternal(Type dataType)
        {
            var result = new List<RichFieldFormat>();

            if (dataType == typeof(DateTime))
            {
                result.AddRange(Formats.Values);
            }
            else if (dataType == typeof(DateOnly))
            {
                AddIfExists(result, RichFieldDateTimeFormatType.ShortDate);
                AddIfExists(result, RichFieldDateTimeFormatType.MediumDate);
                AddIfExists(result, RichFieldDateTimeFormatType.LongDate);
                AddIfExists(result, RichFieldDateTimeFormatType.FullDate);
                AddIfExists(result, RichFieldDateTimeFormatType.MonthYear);
                AddIfExists(result, RichFieldDateTimeFormatType.DayMonth);
                AddIfExists(result, RichFieldDateTimeFormatType.MonthDay);
                AddIfExists(result, RichFieldDateTimeFormatType.YearMonth);
                AddIfExists(result, RichFieldDateTimeFormatType.DayOfWeek);
                AddIfExists(result, RichFieldDateTimeFormatType.ShortDayOfWeek);
            }
            else if (dataType == typeof(TimeOnly))
            {
                AddIfExists(result, RichFieldDateTimeFormatType.ShortTime);
                AddIfExists(result, RichFieldDateTimeFormatType.MediumTime);
                AddIfExists(result, RichFieldDateTimeFormatType.LongTime);
                AddIfExists(result, RichFieldDateTimeFormatType.FullTime);
                AddIfExists(result, RichFieldDateTimeFormatType.HourMinute24);
                AddIfExists(result, RichFieldDateTimeFormatType.HourMinute12);
            }

            return result;
        }

        private void AddIfExists(List<RichFieldFormat> list, RichFieldDateTimeFormatType formatId)
        {
            if (Formats.TryGetValue(formatId, out var format))
                list.Add(format);
        }
    }
}
