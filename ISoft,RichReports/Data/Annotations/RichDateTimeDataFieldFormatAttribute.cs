using ISoft.RichReports.Editor.Fields.Format;

namespace ISoft.RichReports.Data.DataAnnotations
{
    public class RichDateTimeDataFieldFormatAttribute : RichDataFieldFormatAttributeBase
    {
        private readonly RichFieldDateTimeFormatType _formatType;

        public RichDateTimeDataFieldFormatAttribute(RichFieldDateTimeFormatType formatType)
        {
            _formatType = formatType;
        }

        public override RichFieldFormat? GetFormat()
        {
            return RichFieldFormatHelper.GetDateTimeFormat(_formatType);
        }
    }
}
