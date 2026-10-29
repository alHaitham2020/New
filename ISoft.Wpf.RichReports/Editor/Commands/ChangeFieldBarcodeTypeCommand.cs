using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.Office.Barcode;
using ISoft.RichReportDesigner.Core.Fields.Instructions;
using ISoft.RichReportDesigner.Core.RichEdit.Fields;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class ChangeFieldBarcodeTypeCommand : ChangeFieldSwitchValueCommand<BarcodeType>
    {
        public ChangeFieldBarcodeTypeCommand(IRichEditControl control) : base(control) { }

        public ChangeFieldBarcodeTypeCommand(IRichEditControl control, Field field) : base(control, field) { }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldBarcodeType;

        protected override string SwitchKey => RichFieldSwitchKeys.MailMergeField.Barcode;

        protected override BarcodeType DefaultValue =>  BarcodeType.None;

        protected override bool CanChangeFieldCode(Field field)
        {
            if (field == null) return false;

            return field.Parent == null && 
                  (field.CachedFieldType == MergefieldField.FieldType || 
                   field.CachedFieldType == RichExpressionEditorCalculatedField.FieldType); // not nested field 
        }

        protected override void ChangeValueCore(IValueBasedCommandUIState<BarcodeType> state)
        {
            UpdateSwitchValue(Field, state);
        }
    }

}
