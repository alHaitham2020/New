using ISoft.RichReports.Editor.Fields.Formatting;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public interface IRichFieldSwitchBuilder
    {
        IRichFieldSwitch CreateDateTimeFormatSwitch(string formatValue);

        IRichFieldSwitch CreateGeneralFormatSwitch(string formatValue);

        IRichFieldSwitch CreateNumericFormatSwitch(string formatValue);

        IRichFieldSwitch CreateFieldSwitch(string switchKey, string switchValue);

        /// <summary>
        /// ReadOnly Switch ex:(bool) thats dont needs a value
        /// </summary>
        /// <param name="switchKey"></param>
        /// <returns></returns>
        IRichFieldSwitch CreateFieldSwitch(string switchKey);

        IRichFieldSwitch CreateFormatSwitch(RichFieldFormat format);

        IRichFieldSwitch CreateFormatSwitch(RichFieldFormatType formatType, string formatValue);

        IRichFieldSwitch CreateSwitch(string switchKey, string switchValue);
    }
}
