using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Internal.Fields;
using ISoft.RichReports.Editor.Services.Abstractions;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditTimeField : TimeField, IRichEditField
    {
        private string displayName;

        public new static RichEditTimeField Create()
        {
            return new RichEditTimeField();
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
            displayName = service?.GetFieldDisplayName(RichEditFieldType.TimeField);
        }
    }
}
