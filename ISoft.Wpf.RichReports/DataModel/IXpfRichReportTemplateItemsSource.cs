using ISoft.RichReports.UI.DataModel;
using ISoft.Utils.Data.AsyncData;

namespace ISoft.Wpf.RichReports.DataModel
{
    public interface IXpfRichReportTemplateItemsSource : IRichReportTemplateItemsSource
    {
        IAsyncCollectionDataSourceProvider<IXpfRichReportTemplatePreviewItem> Source { get; }
    }
}
