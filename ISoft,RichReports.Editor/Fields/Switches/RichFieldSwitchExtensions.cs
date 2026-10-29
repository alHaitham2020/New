using ISoft.RichReports.Editor.Fields.Format;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public static class RichFieldSwitchExtensions
    {
        public static RichFieldFormat? GetFormat(this IRichFieldSwitch fieldSwitch)
        {
            return TryGetFormat(fieldSwitch, out var format) ? format : null;
        }

        public static bool TryGetFormat(this IRichFieldSwitch fieldSwitch, out RichFieldFormat resultFormat)
        {
            if (fieldSwitch==null || !fieldSwitch.IsFormatSwitch || !fieldSwitch.IsValid())
            {
                resultFormat = RichFieldFormat.Empty;
                return false;
            }

            return RichFieldFormatManager
                    .Current
                    .TryGetFormatForValue(
                        fieldSwitch.SwitchType.ToFormatType(),
                        fieldSwitch.SwitchValue,
                        out resultFormat);
        }
    }
}
