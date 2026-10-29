using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public class RichDocVariableFieldOptions : RichFormatableFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.DocVariableField;

        [RichFieldArgument(0)]
        public string VariableName { get; set; }

        public static RichDocVariableFieldOptions Create(string variableName, RichFieldFormat? format = null)
        {
            return new RichDocVariableFieldOptions()
            {
                VariableName = variableName,
                Format = format
            };
        }
    }
}
