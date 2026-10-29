using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public class RichExpressionEditorCalculatedFieldOptions : RichBarcodeFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.ExpEditCalculatedField;

        [RichFieldArgument(0)]
        public string FieldName { get; set; }

        public static RichExpressionEditorCalculatedFieldOptions Create(string fieldName, RichFieldFormat? format = null)
        {
            return new RichExpressionEditorCalculatedFieldOptions()
            {
                FieldName = fieldName,
                Format = format
            };
        }
    }
}
