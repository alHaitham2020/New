using System.Windows.Markup;

namespace ISoft.Wpf.RichReports.Markup.Extensions
{
    public abstract class RichFieldFormatsSourceExtensionBase : MarkupExtension
    {
        protected abstract RichFieldFormatType FormatType { get; }

        public sealed override object ProvideValue(IServiceProvider serviceProvider) => GetFormats();

        private RichFieldFormat[] GetFormats() =>
            [RichFieldFormat.Empty, .. RichFieldFormatManager.Current.GetProvider(FormatType).GetAllFormats()];
    }
}
