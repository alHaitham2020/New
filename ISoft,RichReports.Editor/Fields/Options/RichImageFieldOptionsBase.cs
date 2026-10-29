using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public abstract class RichImageFieldOptionsBase : RichFormatableFieldOptionsBase
    {
        [RichFieldSwitch(RichFieldSwitchKeys.ImageBasedField.ImageWidth)]
        public int? ImageWidth { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.ImageBasedField.ImageHeight)]
        public int? ImageHeight { get; set; }
    }
}
