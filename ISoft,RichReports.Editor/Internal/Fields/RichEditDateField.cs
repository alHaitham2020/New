using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Services;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditDateField : DateField, IRichEditField
    {
        private string displayName;

        public new static RichEditDateField Create()
        {
            return new RichEditDateField();
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
            displayName = service?.GetFieldDisplayName(RichEditFieldType.DateField);
        }

        private bool IsEmptyString(object value)
        {
            if (!(value is string text))
            {
                return false;
            }

            return text.Length == 0;
        }

        //protected override UpdateFieldOperationType GetAllowedUpdateFieldTypes(FieldUpdateOnLoadOptions options)
        //{
        //    UpdateFieldOperationType updateFieldOperationType = base.GetAllowedUpdateFieldTypes(options);
        //    if (options != null && options.UpdateDateField)
        //    {
        //        updateFieldOperationType |= UpdateFieldOperationType.Load;
        //    }

        //    return updateFieldOperationType;
        //}
    }

}
