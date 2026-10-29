namespace ISoft.RichReports.Data.Annotations
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
