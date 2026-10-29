using System.Windows.Markup;

namespace ISoft.Wpf.RichReports.Markup.Extensions
{
    [MarkupExtensionReturnType(typeof(IEnumerable<EnumMemberInfo>))]
    public class RichBarcodeTypeSourceExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
           return EnumEditSettingsHelper.GetEnumItemsSource<BarcodeType>(false);
        }
    }
}
