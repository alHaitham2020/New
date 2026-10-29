using DevExpress.Xpf.Core;
using ISoft.RichReportDesigner.UI.Controls;


namespace ISoft.RichReportDesigner.UI.View
{
    /// <summary>
    /// Interaction logic for DXWindow1.xaml
    /// </summary>
    public partial class RichReportDesignerWindow : ThemedWindow
    {
        public RichReportDesignerControl DesignerControl => designer;

        public RichReportDesignerWindow()
        {
            InitializeComponent();
        }
    }

}
