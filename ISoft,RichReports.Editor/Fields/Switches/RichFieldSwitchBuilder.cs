using ISoft.RichReports.Editor.Fields.Formatting;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public class RichFieldSwitchBuilder : IRichFieldSwitchBuilder
    {
        public IRichFieldSwitch CreateSwitch(string switchKey, string switchValue)
        {
            return RichFieldSwitch.CreateSwitch(switchKey, switchValue);
        }

        public IRichFieldSwitch CreateFieldSwitch(string switchKey, string switchValue)
        {
            return RichFieldSwitch.CreateFieldSpecific(switchKey, switchValue);
        }

        /// <summary>
        /// For ReadOnly Switches like (bool) thats dose not need a value
        /// </summary>
        /// <param name="switchKey"></param>
        /// <returns></returns>
        public IRichFieldSwitch CreateFieldSwitch(string switchKey)
        {
            return CreateFieldSwitch(switchKey, null);
        }

        public IRichFieldSwitch CreateDateTimeFormatSwitch(string formatValue)
        {
            return RichFieldSwitch.CreateDateTimeFormat(formatValue);
        }

        public IRichFieldSwitch CreateNumericFormatSwitch(string formatValue)
        {
            return RichFieldSwitch.CreateNumericFormat(formatValue);
        }

        public IRichFieldSwitch CreateGeneralFormatSwitch(string formatValue)
        {
            return RichFieldSwitch.CreateGeneralFormat(formatValue);
        }

        public IRichFieldSwitch CreateFormatSwitch(RichFieldFormatType formatType,string formatValue)
        {
            if (string.IsNullOrEmpty(formatValue))
                return null;


            return formatType switch
            {
                RichFieldFormatType.General => CreateGeneralFormatSwitch(formatValue),
                RichFieldFormatType.Numeric => CreateNumericFormatSwitch(formatValue),
                RichFieldFormatType.DateTime => CreateDateTimeFormatSwitch(formatValue),
                _ => null,
            };
        }

        public IRichFieldSwitch CreateFormatSwitch(RichFieldFormat format)
        {
            if (RichFieldFormat.IsNullOrEmpty(format))
                return null;

            var formatType = format.FormatType;
            var value = format.Value;

            return CreateFormatSwitch(formatType, value);
        }
    }
}
