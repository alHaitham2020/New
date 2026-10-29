using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Formatting
{
    // Non-generic interface for manager
    public interface IRichFieldFormatProvider
    {
        RichFieldFormatType FormatType { get; }

        RichFieldFormat[] GetAllFormats();

        RichFieldFormat? GetFormatByName(string name);

        bool TryGetFormatByName(string name, out RichFieldFormat result);

        string GetDefaultFormatValue(Type dataType);

        bool TryGetDefaultFormat(Type dataType, out RichFieldFormat? result);

        bool TryGetDefaultFormatValue(Type dataType, out string result);

        bool HasDefaultFormatValue(Type dataType);

        RichFieldFormat? GetDefaultFormat(Type dataType);

        List<string> GetAvailableFormatValues(Type dataType);

        List<RichFieldFormat> GetAvailableFormats(Type dataType);

        RichFieldFormatType? GetFormatTypeForValue(string formatValue);

        RichFieldFormat? GetFormatForValue(string formatValue);

        bool TryGetFormatForValue(string formatValue, out RichFieldFormat resultFormat);

    }

    public interface IRichFieldFormatProvider<TFormatId> : IRichFieldFormatProvider where TFormatId : struct
    {
        void Register(TFormatId formatType, string displayName, string value);

        void Unregister(TFormatId formatType);

        RichFieldFormat? GetFormat(TFormatId type);

        bool TryGetFormat(TFormatId formatType, out RichFieldFormat result);

        bool HasFormat(TFormatId formatType);
    }
}
