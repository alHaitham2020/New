using System.Windows.Media;

namespace ISoft.Wpf.RichReports.Models
{
    public interface IXpfRichReportTemplatePreviewItem : IRichReportTemplatePreviewItem
    {
        bool IsLoading { get; }

        ImageSource PreviewImage { get; }
    }
}
