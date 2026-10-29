namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class ChangeFieldImageHeightCommand : ChangeFieldSwitchValueImageDimensionCommandBase<int?>
    {
        public ChangeFieldImageHeightCommand(IRichEditControl control) : base(control) { }
        public ChangeFieldImageHeightCommand(IRichEditControl control, Field field) : base(control, field) { }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldImageHeight;

        protected override string SwitchKey => RichFieldSwitchKeys.ImageBasedField.ImageHeight;

        protected override int? DefaultValue => 0;
    }
}
