namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public abstract class FieldBasedCommand : FieldBasedRichEditMenuItemSimpleCommand
    {
        private RichDocumentModelFieldHelper _helper;

        protected internal RichDocumentModelFieldHelper Helper => _helper ??= new RichDocumentModelFieldHelper(DocumentModel);

        protected static IRichFieldCodeScanner Scanner => RichFieldCodeScanner.Default;

        protected FieldBasedCommand(IRichEditControl control) : base(control, null) { }

        protected FieldBasedCommand(IRichEditControl control, Field field) : base(control, field) { }

        #region Ovverieds

        public override XtraRichEditStringId MenuCaptionStringId => XtraRichEditStringId.MenuCmd_None;

        public override XtraRichEditStringId DescriptionStringId => XtraRichEditStringId.MenuCmd_None;

        protected new IRichEditorControl Control => base.Control as IRichEditorControl;

        public sealed override void ForceExecute(ICommandUIState state)
        {
            CheckExecutedAtUIThread();
            NotifyBeginCommandExecution(state);
            try
            {
                if (ValidateField())
                {
                    ExecuteCore(state);
                }
            }
            finally
            {
                NotifyEndCommandExecution(state);
            }
        }

        protected abstract void ExecuteCore(ICommandUIState state);       

        protected sealed override void ExecuteCore() { }

        protected override void UpdateUIStateCore(ICommandUIState state)
        {
            CheckExecutedAtUIThread();
            ValidateField();
            OnApplyCommandRestriction(state);
        }

        #endregion

        #region Protetcted Actions
        protected virtual void OnApplyCommandRestriction(ICommandUIState state)
        {
            ApplyCommandsRestriction(state, DocumentModel.DocumentCapabilities.Fields, CanAllowCommand(Field));
        }

        protected IRichFieldCode ScanFieldCode(Field field)
        {
            if (field == null) return null;

            var fieldCode = Scanner.Scan(Helper.GetCleanCode(field));// only scan with clean code
            if (fieldCode == null) return null;

            return fieldCode;
        }

        #endregion

        #region Abstract
        protected virtual bool CanAllowCommand(Field field) => true;
        #endregion
    }
}
