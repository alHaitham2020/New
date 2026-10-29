using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Services;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditPageField : PageField, IRichEditField
    {
        private string displayName;

        public new static RichEditPageField Create()
        {
            return new RichEditPageField();
        }

        protected override FieldMailMergeType MailMergeType()
        {
            return FieldMailMergeType.MailMerge;
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            return this.CalculateDisplayName(displayName, FieldType);
        }

        public override void Initialize(PieceTable pieceTable, InstructionCollection switches)
        {
            base.Initialize(pieceTable, switches);

            IRichEditFieldsManagerService service = pieceTable.DocumentModel.GetService<IRichEditFieldsManagerService>();
            displayName = service?.GetFieldDisplayName(RichEditFieldType.PageField);
        }
    }
}
