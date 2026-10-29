using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Services;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditPrintDateField : PrintDateField, IRichEditField
    {
        private string _displayName;

        public new static RichEditPrintDateField Create()
        {
            return new RichEditPrintDateField();
        }

        protected override FieldMailMergeType MailMergeType()
        {
            return FieldMailMergeType.MailMerge;
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            return this.CalculateDisplayName(_displayName, FieldType);
        }

        public override void Initialize(PieceTable pieceTable, InstructionCollection switches)
        {
            base.Initialize(pieceTable, switches);

            IRichEditFieldsManagerService service = pieceTable.DocumentModel.GetService<IRichEditFieldsManagerService>();
            _displayName = service?.GetFieldDisplayName(RichEditFieldType.PrintDateField);
        }
    }
}
