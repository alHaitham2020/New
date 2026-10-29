using DevExpress.Mvvm.UI.Native;
using ISoft.RichReports.Designer.Fields;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ISoft.Wpf.RichReports.Controls
{
    public class RichReportDesignerFieldsPanel : Control
    {
        public static readonly DependencyProperty FieldsSourceProperty;

        static RichReportDesignerFieldsPanel()
        {
            DependencyPropertyRegistrator<RichReportDesignerFieldsPanel>
                .New()
                .OverrideDefaultStyleKey()
                .Register<IEnumerable<IRichDisplayFieldGroup>>(nameof(FieldsSource), out FieldsSourceProperty, null);
        }

        public IEnumerable<IRichDisplayFieldGroup> FieldsSource
        {
            get => (IEnumerable<IRichDisplayFieldGroup>)GetValue(FieldsSourceProperty);
            set => SetValue(FieldsSourceProperty, value);
        }
    }
}
