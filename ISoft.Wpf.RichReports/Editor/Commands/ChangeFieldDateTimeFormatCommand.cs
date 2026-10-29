namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class ChangeFieldDateTimeFormatCommand : ChangeFieldFormatCommandBase
    {
        public ChangeFieldDateTimeFormatCommand(IRichEditControl control) : base(control)
        {
        }

        public ChangeFieldDateTimeFormatCommand(IRichEditControl control, Field field) : base(control, field)
        {

        }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldDateTimeFormat;

        protected override string SwitchKey => RichStandardFieldSwitchKeys.DateTimeKey;

        protected override RichFieldDataTypeCategory DataTypeCategory => RichFieldDataTypeCategory.DateTime;
    }
}
