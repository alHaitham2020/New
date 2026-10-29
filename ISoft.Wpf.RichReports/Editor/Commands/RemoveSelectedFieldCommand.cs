using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class RemoveSelectedFieldCommand : FieldBasedCommand
    {
        public RemoveSelectedFieldCommand(IRichEditControl control)
            : this(control, null)
        {
            
        }

        public RemoveSelectedFieldCommand(IRichEditControl control, DevExpress.XtraRichEdit.Model.Field field)
            : base(control, field)
        {
            
        }

        public override RichEditCommandId Id => XpfRichEditCommandId.RemoveSelectedField;

        protected override void ExecuteCore(ICommandUIState state)
        {
            Helper.DeleteFieldCode(Field);
        }

        protected override void UpdateUIStateCore(ICommandUIState state)
        {
            CheckExecutedAtUIThread();
            ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Fields);
            if (state.Enabled && DocumentModel.FieldOptions.UpdateLockedFields != UpdateLockedFields.Always)
            {
                ValidateField();
                if (Field != null)
                {
                    state.Enabled &= !Field.Locked;
                }
                else
                {
                    state.Enabled = false;
                }
            }
        }
    }

}
