namespace ISoft.Wpf.RichReports.Models
{
    public interface IXpfRichReportTemplateItemsSource : IRichReportTemplateItemsSource
    {
        IAsyncCollectionDataSourceProvider<IXpfRichReportTemplatePreviewItem> Source { get; }
    }
}
