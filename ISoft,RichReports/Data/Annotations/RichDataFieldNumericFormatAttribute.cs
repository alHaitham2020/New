using ISoft.RichReports.Editor.Fields.Format;

namespace ISoft.RichReports.Data.DataAnnotations
{
    public class RichDataFieldNumericFormatAttribute : RichDataFieldFormatAttributeBase
    {
        private readonly RichFieldNumericFormatType _formatType;

        public RichDataFieldNumericFormatAttribute(RichFieldNumericFormatType formatType)
        {
            _formatType = formatType;
        }

        public override RichFieldFormat? GetFormat()
        {
            return RichFieldFormatHelper.GetNumericFormat(_formatType);
        }
    }
}
