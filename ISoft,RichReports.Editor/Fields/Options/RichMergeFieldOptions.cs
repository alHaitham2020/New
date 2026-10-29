using ISoft.RichReports.Editor.Fields.Codes;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public class RichMergeFieldOptions : RichBarcodeFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.MergeField;

        [RichFieldArgument(0)]
        public string DataFieldPath { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.MailMergeField.Before)]
        public string StringBefore { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.MailMergeField.After)]
        public string StringAfter { get; set; }

        //public override void Write(IRichFieldCode fieldCode)
        //{
        //    if (!string.IsNullOrWhiteSpace(DataFieldPath))
        //    {
        //        fieldCode.Arguments.AddValueAtIndex(0, DataFieldPath);
        //    }

        //    if (!string.IsNullOrWhiteSpace(StringBefore))
        //    {
        //        fieldCode.Switches.AddOrUpdateSwitch(RichFieldSwitchKeys.MailMergeField.Before, StringBefore);
        //    }

        //    if (!string.IsNullOrWhiteSpace(StringAfter))
        //    {
        //        fieldCode.Switches.AddOrUpdateSwitch(RichFieldSwitchKeys.MailMergeField.After, StringAfter);
        //    }
        //}

        //public override void Read(IRichFieldCode fieldCode)
        //{
        //    DataFieldPath = fieldCode.Arguments.GetFirstValue();
        //    StringBefore = fieldCode.Switches.GetSwitchValue(RichFieldSwitchKeys.MailMergeField.Before);
        //    StringAfter = fieldCode.Switches.GetSwitchValue(RichFieldSwitchKeys.MailMergeField.After);
        //}

        public static RichMergeFieldOptions Create(string dataFieldPath, RichFieldFormat? format = null)
        {
            return new RichMergeFieldOptions() 
            {
                DataFieldPath = dataFieldPath ,
                Format = format
            };
        }
    }
}
