using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class RichShowCalculatedFieldsEditorFormCommand : RichEditMenuItemSimpleCommandBase
    {
        public RichShowCalculatedFieldsEditorFormCommand(IRichEditControl control) : base(control)
        {

        }

        public override RichEditCommandId Id => XpfRichEditCommandId.RichShowCalculatedFieldsEditorForm;

        protected override void ExecuteCore()
        {
            Control.ShowCalculatedFieldsForm();
        }
    }
}
