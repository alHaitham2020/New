using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Formatting;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public class RichDocCustomPropertyFieldOptions : RichFormatableFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.CustomPropertyField;

        [RichFieldArgument(0)]
        public string PropertyName { get; set; }

        public static RichDocCustomPropertyFieldOptions Create(string propertyName, RichFieldFormat? format = null)
        {
            return new RichDocCustomPropertyFieldOptions()
            {
                PropertyName = propertyName,
                Format = format
            };
        }
    }
}
