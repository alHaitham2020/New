namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public abstract class RichEditMenuItemSimpleCommandBase : RichEditMenuItemSimpleCommand
    {
        public RichEditMenuItemSimpleCommandBase(IRichEditControl control) : base(control)
        {

        }

        public override XtraRichEditStringId MenuCaptionStringId => XtraRichEditStringId.MenuCmd_None;

        public override XtraRichEditStringId DescriptionStringId => XtraRichEditStringId.MenuCmd_None;

        protected new IRichEditorControl Control => base.Control as IRichEditorControl;

        protected override void UpdateUIStateCore(ICommandUIState state)
        {
            CheckExecutedAtUIThread();
            ApplyCommandsRestriction(state, Options.DocumentCapabilities.Fields);
            //state.Enabled = state.Enabled && HyperlinkSource != null;
        }
    }
}
