namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class ChangeFieldNumericFormatCommand : ChangeFieldFormatCommandBase
    {
        public ChangeFieldNumericFormatCommand(IRichEditControl control) : base(control) { }

        public ChangeFieldNumericFormatCommand(IRichEditControl control, Field field) : base(control, field) { }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldNumericFormat;

        protected override string SwitchKey => RichStandardFieldSwitchKeys.NumericKey;

        protected override RichFieldDataTypeCategory DataTypeCategory => RichFieldDataTypeCategory.Numeric;
    }
}
