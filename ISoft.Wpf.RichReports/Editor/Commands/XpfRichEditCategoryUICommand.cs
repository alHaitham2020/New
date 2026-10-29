using DevExpress.Xpf.RichEdit.Internal;
using DevExpress.XtraRichEdit.Commands;
using ISoft.RichReportDesigner.UI.Controls;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class XpfRichEditCategoryUICommand : RichEditCategoryUICommand
    {
        public XpfRichEditCategoryUICommand(XpfRichEditorControl richEditControl, RichEditCommandId id)
            : base(richEditControl, id)
        {

        }

        protected new internal void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();
        }
    }
}
