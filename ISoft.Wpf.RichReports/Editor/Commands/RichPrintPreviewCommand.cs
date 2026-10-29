namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class RichPrintPreviewCommand : PrintPreviewCommand
    {
        public RichPrintPreviewCommand(IRichEditControl control) : base(control)
        {

        }

        public new IRichEditorControl Control => base.Control as IRichEditorControl;

        protected override void ExecuteCore()
        {
            CheckExecutedAtUIThread();
            Control.ShowPreviewAsync();

            //_designerControl.ActualCommands.ShowPreviewCommand.TryExecuteCommand(null);
        }

        protected override void UpdateUIStateCore(ICommandUIState state)
        {
            CheckExecutedAtUIThread();
            ApplyCommandsRestriction(state, Options.Behavior.Printing, Control.IsPrintingAvailable && Control.IsPrintPreviewAvailable && Control.CanShowPreview());
        }
    }
}
