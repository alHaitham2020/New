using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.Commands;
using ISoft.RichReportDesigner.UI.Controls;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class XpfRichEditUICommand : RichEditUICommand
    {
        protected new XpfRichEditorControl Control => base.Control as XpfRichEditorControl;

        public XpfRichEditUICommand()
        {
        }

        protected internal XpfRichEditUICommand(RichEditCommandId id) : base(id)
        {

        }

        protected internal XpfRichEditUICommand(XpfRichEditorControl richEditControl, RichEditCommandId id) : base(richEditControl, id)
        {

        }

        protected internal XpfRichEditUICommand(RichEditCommandId id, object parameter) : base(id, parameter)
        {

        }

        protected internal XpfRichEditUICommand(XpfRichEditorControl richEditControl, RichEditCommandId id, object parameter) : base(richEditControl, id, parameter)
        {

        }

        protected new internal void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();
        }
    }
}
