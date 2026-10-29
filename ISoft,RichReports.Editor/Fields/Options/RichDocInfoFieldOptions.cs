using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public class RichDocInfoFieldOptions : RichFormatableFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.InfoField;

        [RichFieldArgument(0)]
        public string PropertyName { get; set; }

        public static RichDocInfoFieldOptions Create(string propertyName, RichFieldFormat? format = null)
        {
            return new RichDocInfoFieldOptions()
            {
                PropertyName = propertyName,
                Format = format
            };
        }
    }
}
