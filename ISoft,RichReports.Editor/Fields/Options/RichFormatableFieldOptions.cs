using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public class RichFormatableFieldOptions : RichFormatableFieldOptionsBase
    {
        private readonly RichEditFieldType _fieldType;

        public override RichEditFieldType FieldType => _fieldType;

        public RichFormatableFieldOptions(RichEditFieldType fieldType, RichFieldFormat? format = null)
        {
            _fieldType = fieldType;
            Format = format;
        }

        public static RichFormatableFieldOptions Create(RichEditFieldType fieldType,RichFieldFormat? format = null)
        {
            return new RichFormatableFieldOptions(fieldType, format);
        }
    }
}
