namespace ISoft.Wpf.RichReports.Views
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
