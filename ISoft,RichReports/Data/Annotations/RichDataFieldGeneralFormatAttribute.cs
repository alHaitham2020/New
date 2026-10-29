using ISoft.RichReports.Editor.Fields.Format;

namespace ISoft.RichReports.Data.DataAnnotations
{
    public class RichDataFieldGeneralFormatAttribute : RichDataFieldFormatAttributeBase
    {
        private readonly RichFieldGeneralFormatType _formatType;

        public RichDataFieldGeneralFormatAttribute(RichFieldGeneralFormatType formatType)
        {
            _formatType = formatType;
        }

        public override RichFieldFormat? GetFormat()
        {
            return RichFieldFormatHelper.GetGeneralFormat(_formatType);
        }
    }
}
