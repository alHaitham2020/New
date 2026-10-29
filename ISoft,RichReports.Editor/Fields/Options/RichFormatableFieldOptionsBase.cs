using ISoft.RichReports.Editor.Fields.Formatting;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public abstract class RichFormatableFieldOptionsBase : RichFieldOptionsBase
    {
        [RichFieldFormatSwitch]
        public RichFieldFormat? Format { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.GeneralFormat.MERGEFORMAT)]
        public bool PreserveFormatting { get; set; }
    }
}
