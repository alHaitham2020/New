using ISoft.RichReports.Editor.Internal.Fields;
using ISoft.RichReports.Editor.Services.Abstractions;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditDocumentPropertyField : DocumentPropertyField, IRichEditField
    {
        private string _displayName;

        public new static CalculatedFieldBase Create()
        {
            return new RichEditDocumentPropertyField();
        }

        protected override FieldMailMergeType MailMergeType()
        {
            return FieldMailMergeType.MailMerge;
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            string fieldDisplayName = this.CalculateDisplayName(_displayName, PropertyName);

            if (string.IsNullOrWhiteSpace(fieldDisplayName))
            {
                return GetDefaultInvalidValueText();
            }

            return fieldDisplayName;
        }

        public override void Initialize(PieceTable pieceTable, InstructionCollection switches)
        {
            base.Initialize(pieceTable, switches);

            IRichEditDocumentPropertiesManagerService service = pieceTable.DocumentModel.GetService<IRichEditDocumentPropertiesManagerService>();
            _displayName = service?.GetDisplayName(PropertyName);
        }
    }
}
