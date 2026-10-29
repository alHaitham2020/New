using ISoft.RichReports.Editor.Fields.Format;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public interface IRichFieldFluentSwitchBuilder
    {
        IRichFieldFluentSwitchBuilder AddDateTimeFormat(string formatValue);

        IRichFieldFluentSwitchBuilder AddGeneralFormat(string formatValue);

        IRichFieldFluentSwitchBuilder AddNumericFormat(string formatValue);

        IRichFieldFluentSwitchBuilder AddFieldSwitch(string switchCode, string switchValue);

        IRichFieldFluentSwitchBuilder AddFieldSwitch(string switchCode);

        IRichFieldFluentSwitchBuilder AddFieldFormat(RichFieldFormat format);

        IRichFieldFluentSwitchBuilder AddGeneralFormat(RichFieldGeneralFormatType formatType);

        IRichFieldFluentSwitchBuilder AddNumericFormat(RichFieldNumericFormatType formatType);

        IRichFieldFluentSwitchBuilder AddDateTimeFormat(RichFieldDateTimeFormatType formatType);

        IRichFieldFluentSwitchBuilder Remove(string switchCode);

        IRichFieldSwitchReadOnlyCollection Build();

        string BuildString();

        
    }
}
