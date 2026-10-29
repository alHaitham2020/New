using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public class RichDocPropertyFieldOptions : RichFormatableFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.DocPropertyField;

        [RichFieldArgument(0)]
        public string PropertyName { get; set; }

        public static RichDocPropertyFieldOptions Create(string propertyName, RichFieldFormat? format = null)
        {
            return new RichDocPropertyFieldOptions()
            {
                PropertyName = propertyName,
                Format = format
            };
        }
    }
}
