using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Formatting
{
    public enum RichFieldNumericFormatType
    {
        // ── Integer ────────────────────────────────────────────────
        Integer,              // #,##0
        IntegerNoGrouping,    // 0

        // ── Decimal ────────────────────────────────────────────────
        Decimal,              // #,##0.00
        DecimalOne,           // #,##0.0
        DecimalThree,         // #,##0.000
        DecimalNoGrouping,    // 0.00
        DecimalTrailingZeros, // 0.00## (keeps significant decimals)

        // ── Currency ───────────────────────────────────────────────
        Currency,             // $#,##0.00
        CurrencyCode,         // USD #,##0.00
        CurrencyNoDecimals,   // $#,##0
        CurrencyAccounting,   // $#,##0.00;($#,##0.00)

        // ── Percent ────────────────────────────────────────────────
        Percent,              // #,##0%
        PercentDecimal,       // #,##0.00%
        PercentNoGrouping,    // 0%
        PerMille,             // #,##0‰

        Custom,               // user-defined pattern
    }

    public interface IRichFieldNumericFormatProvider : IRichFieldFormatProvider<RichFieldNumericFormatType>
    {

    }

    public class RichFieldNumericFormatProvider : RichFieldFormatProviderBase<RichFieldNumericFormatType>, IRichFieldNumericFormatProvider
    {
        public override RichFieldFormatType FormatType => RichFieldFormatType.Numeric;

        public RichFieldNumericFormatProvider()
        {

        }

        protected override void PopulateFormats()
        {
            // ── Integer ────────────────────────────────────────────────
            Register(RichFieldNumericFormatType.Integer,RichFieldFormat.Numeric("Integer", "#,##0", "Integer"));
            Register(RichFieldNumericFormatType.IntegerNoGrouping, RichFieldFormat.Numeric("IntegerNoGrouping", "0", "Integer no grouping"));

            // ── Decimal ────────────────────────────────────────────────
            Register(RichFieldNumericFormatType.Decimal,RichFieldFormat.Numeric("Decimal", "#,##0.00", "Decimal"));
            Register(RichFieldNumericFormatType.DecimalOne,RichFieldFormat.Numeric("DecimalOne", "#,##0.0", "Decimal 1 place"));
            Register(RichFieldNumericFormatType.DecimalThree,RichFieldFormat.Numeric("DecimalThree", "#,##0.000", "Decimal 3 places"));
            Register(RichFieldNumericFormatType.DecimalNoGrouping,RichFieldFormat.Numeric("DecimalNoGrouping", "0.00", "Decimal no grouping"));
            Register(RichFieldNumericFormatType.DecimalTrailingZeros,RichFieldFormat.Numeric("DecimalTrailingZeros", "0.00##", "Decimal trailing"));

            // ── Currency ───────────────────────────────────────────────
            Register(RichFieldNumericFormatType.Currency,RichFieldFormat.Numeric("Currency", "$#,##0.00", "Currency"));
            Register(RichFieldNumericFormatType.CurrencyCode,RichFieldFormat.Numeric("CurrencyCode", "USD #,##0.00", "Currency code"));
            Register(RichFieldNumericFormatType.CurrencyNoDecimals,RichFieldFormat.Numeric("CurrencyNoDecimals", "$#,##0", "Currency no decimals"));
            Register(RichFieldNumericFormatType.CurrencyAccounting,RichFieldFormat.Numeric("CurrencyAccounting", "$#,##0.00;($#,##0.00)", "Currency accounting"));

            // ── Percent ────────────────────────────────────────────────
            Register(RichFieldNumericFormatType.Percent,RichFieldFormat.Numeric("Percent", "#,##0%", "Percent"));
            Register(RichFieldNumericFormatType.PercentDecimal,RichFieldFormat.Numeric("PercentDecimal", "#,##0.00%", "Percent 2 places"));
            Register(RichFieldNumericFormatType.PercentNoGrouping,RichFieldFormat.Numeric("PercentNoGrouping", "0%", "Percent no grouping"));
            Register(RichFieldNumericFormatType.PerMille, RichFieldFormat.Numeric("PerMille", "#,##0‰", "Per Mille"));

            // ── Custom ───────────────────────────────
            Register(RichFieldNumericFormatType.Custom, RichFieldFormat.Numeric("Custom", "{0}", "Custom"));
        }

        protected override void PopulateDefaultFormats()
        {
            // Integer types
            RegisterDefaultFormat(typeof(int), RichFieldNumericFormatType.IntegerNoGrouping);
            RegisterDefaultFormat(typeof(long), RichFieldNumericFormatType.IntegerNoGrouping);
            RegisterDefaultFormat(typeof(short), RichFieldNumericFormatType.IntegerNoGrouping);
            RegisterDefaultFormat(typeof(byte), RichFieldNumericFormatType.IntegerNoGrouping);
            RegisterDefaultFormat(typeof(uint), RichFieldNumericFormatType.IntegerNoGrouping);
            RegisterDefaultFormat(typeof(ulong), RichFieldNumericFormatType.IntegerNoGrouping);
            RegisterDefaultFormat(typeof(ushort), RichFieldNumericFormatType.IntegerNoGrouping);
            RegisterDefaultFormat(typeof(sbyte), RichFieldNumericFormatType.IntegerNoGrouping);

            // Floating-point types
            RegisterDefaultFormat(typeof(decimal), RichFieldNumericFormatType.Decimal);
            RegisterDefaultFormat(typeof(double), RichFieldNumericFormatType.Decimal);
            RegisterDefaultFormat(typeof(float), RichFieldNumericFormatType.Decimal);
        }

        protected override List<RichFieldFormat> GetAvailableFormatsInternal(Type dataType)
        {
            var result = new List<RichFieldFormat>();

            if (IsFloatingPointType(dataType))
            {
                // Floating-point supports everything
                result.AddRange(Formats.Values);
            }
            else if (IsIntegerType(dataType))
            {
                AddIfExists(result, RichFieldNumericFormatType.Integer);
                AddIfExists(result, RichFieldNumericFormatType.IntegerNoGrouping);
                AddIfExists(result, RichFieldNumericFormatType.Decimal);
                AddIfExists(result, RichFieldNumericFormatType.Currency);
                AddIfExists(result, RichFieldNumericFormatType.CurrencyNoDecimals);
                AddIfExists(result, RichFieldNumericFormatType.Percent);
            }

            return result;
        }

        private void AddIfExists(List<RichFieldFormat> list, RichFieldNumericFormatType formatId)
        {
            if (Formats.TryGetValue(formatId, out var format))
                list.Add(format);
        }

        private static bool IsIntegerType(Type type) =>
            type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte);

        private static bool IsFloatingPointType(Type type) =>
            type == typeof(decimal) || type == typeof(double) || type == typeof(float);
    }
}
