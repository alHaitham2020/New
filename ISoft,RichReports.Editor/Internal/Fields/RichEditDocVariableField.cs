using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReports.Editor.Services;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditDocVariableField : DocVariableField, IRichEditField
    {
        private string _displayName;
        private string variableName;

        public new static CalculatedFieldBase Create()
        {
            return new RichEditDocVariableField();
        }

        protected override FieldMailMergeType MailMergeType()
        {
            return FieldMailMergeType.MailMerge;
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            string fieldDisplayName = this.CalculateDisplayName(_displayName, variableName);

            if (string.IsNullOrWhiteSpace(fieldDisplayName))
            {
                return GetDefaultInvalidValueText();
            }

            return fieldDisplayName;
        }

        public override void Initialize(PieceTable pieceTable, InstructionCollection instructions)
        {
            base.Initialize(pieceTable, instructions);
            variableName = instructions.GetArgumentAsString(0);

            IRichEditDocumentVariableManagerService service = pieceTable.DocumentModel.GetService<IRichEditDocumentVariableManagerService>();
            _displayName = service?.GetDisplayName(variableName);
        }
    }
}
