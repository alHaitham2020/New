namespace ISoft.Wpf.RichReports.Editor.Commands
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
