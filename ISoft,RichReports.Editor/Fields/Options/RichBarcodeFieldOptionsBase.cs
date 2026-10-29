using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public abstract class RichBarcodeFieldOptionsBase : RichImageFieldOptionsBase
    {
        [RichFieldSwitch(RichFieldSwitchKeys.MailMergeField.Barcode)]
        [DefaultNullValue(BarcodeType.None)]
        public BarcodeType BarcodeType { get; set; } = BarcodeType.None;
    }
}
