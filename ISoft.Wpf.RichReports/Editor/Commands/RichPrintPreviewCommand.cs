using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using ISoft.RichReportDesigner.Core.RichEdit;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
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
