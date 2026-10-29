namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class RichSaveDocumentCommand : SaveDocumentCommand
    {
        public new IRichEditorControl Control => base.Control as IRichEditorControl;

        public RichSaveDocumentCommand(IRichEditControl control) : base(control)
        {

        }

        protected override void ExecuteCore()
        {
            CheckExecutedAtUIThread();
            Control.SaveAsync();
        }

        protected override void UpdateUIStateCore(ICommandUIState state)
        {
            CheckExecutedAtUIThread();
            ApplyCommandsRestriction(state, Options.Behavior.Save, DocumentServer.Modified && Control.CanSave());
        }
    }
}
