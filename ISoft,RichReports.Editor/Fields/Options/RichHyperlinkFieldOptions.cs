using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Formatting;

namespace ISoft.RichReports.Editor.Fields.Options
{
    /// <summary>
    /// Options for HYPERLINK fields
    /// </summary>
    public class RichHyperlinkFieldOptions : RichFormatableFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.HyperlinkField;

        [RichFieldArgument(0)]
        public string Url { get; set; }

        public static RichHyperlinkFieldOptions Create(string url, RichFieldFormat? format = null)
        {
            return new RichHyperlinkFieldOptions()
            {
                Url = url,
                Format = format
            };
        }
    }
}
