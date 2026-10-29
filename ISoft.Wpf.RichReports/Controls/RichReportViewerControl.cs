using System.Windows;
using System.Windows.Controls;

namespace ISoft.Wpf.RichReports.Controls
{
    public class RichReportViewerControl : Control
    {
        public static readonly DependencyProperty DocumentSourceProperty;

        static RichReportViewerControl()
        {
            DependencyPropertyRegistrator<RichReportViewerControl>
                .New()
                .OverrideDefaultStyleKey()
                .OverrideMetadata(VerticalContentAlignmentProperty, VerticalAlignment.Stretch)
                .OverrideMetadata(HorizontalContentAlignmentProperty, HorizontalAlignment.Stretch)
                .OverrideMetadata(SnapsToDevicePixelsProperty, true)
                .Register<object>(nameof(DocumentSource), out DocumentSourceProperty, null);
        }

        public object DocumentSource
        {
            get => GetValue(DocumentSourceProperty);
            set => SetValue(DocumentSourceProperty, value);
        }

        #region Ovverieds
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
        #endregion
    }
}
