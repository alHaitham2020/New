using ISoft.RichReports.UI.DataModel;
using System.Windows.Media;

namespace ISoft.Wpf.RichReports.DataModel
{
    public interface IXpfRichReportTemplatePreviewItem : IRichReportTemplatePreviewItem
    {
        bool IsLoading { get; }

        ImageSource PreviewImage { get; }
    }
}
