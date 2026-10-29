namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class ChangeFieldImageWidthCommand : ChangeFieldSwitchValueImageDimensionCommandBase<int?>
    {
        public ChangeFieldImageWidthCommand(IRichEditControl control) : base(control) { }
        public ChangeFieldImageWidthCommand(IRichEditControl control, Field field) : base(control, field) { }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldImageWidth;

        protected override string SwitchKey => RichFieldSwitchKeys.ImageBasedField.ImageWidth;

        protected override int? DefaultValue => 0;
    }

}
