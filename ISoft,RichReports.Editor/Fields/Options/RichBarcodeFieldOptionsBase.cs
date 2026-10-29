using ISoft.Office.Barcode;
using ISoft.RichReports.Editor.Fields.Instructions;
using ISoft.Utils.DataAnnotations;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public abstract class RichBarcodeFieldOptionsBase : RichImageFieldOptionsBase
    {
        [RichFieldSwitch(RichFieldSwitchKeys.MailMergeField.Barcode)]
        [DefaultNullValue(BarcodeType.None)]
        public BarcodeType BarcodeType { get; set; } = BarcodeType.None;
    }
}
