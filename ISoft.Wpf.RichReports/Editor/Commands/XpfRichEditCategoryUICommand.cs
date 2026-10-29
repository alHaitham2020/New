using ISoft.Wpf.RichReports.Controls;

namespace ISoft.Wpf.RichReports.Editor.Commands
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
