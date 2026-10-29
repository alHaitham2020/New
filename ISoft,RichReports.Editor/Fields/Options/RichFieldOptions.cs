using ISoft.RichReports.Editor.Fields.Core;

namespace ISoft.RichReports.Editor.Fields.Options
{
    /// <summary>
    /// empty options with none arguments and none switches
    /// </summary>
    public class RichFieldOptions : RichFieldOptionsBase
    {
        private readonly RichEditFieldType _fieldType;

        public override RichEditFieldType FieldType => _fieldType;

        public RichFieldOptions(RichEditFieldType fieldType)
        {
            _fieldType = fieldType;
        }

        public static RichFieldOptions Create(RichEditFieldType fieldType)
        {
            return new RichFieldOptions(fieldType);
        }
    }
}
