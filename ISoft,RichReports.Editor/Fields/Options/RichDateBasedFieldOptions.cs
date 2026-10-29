using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Formatting;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    /// <summary>
    /// Options for Date based fields
    /// </summary>
    public class RichDateBasedFieldOptions : RichFormatableFieldOptionsBase
    {
        private readonly RichEditFieldType _fieldType;

        public override RichEditFieldType FieldType => _fieldType;

        [RichFieldSwitch(RichFieldSwitchKeys.DateField.UseLunarCalendar)]
        public bool UseLunarCalendar { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.DateField.UseSakaEraCalendar)]
        public bool UseSakaEraCalendar { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.DateField.UseLastFormat)]
        public bool UseLastFormat { get; set; }

        public RichDateBasedFieldOptions(RichEditFieldType fieldType, RichFieldFormat? format = null)
        {
            _fieldType = fieldType;
            Format = format;
        }

        public static RichDateBasedFieldOptions Create(RichEditFieldType fieldType, RichFieldFormat? format = null)
        {
            return new RichDateBasedFieldOptions(fieldType, format);
        }
    }
}
