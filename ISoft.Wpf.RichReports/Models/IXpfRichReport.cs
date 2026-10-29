namespace ISoft.Wpf.RichReports.Models
{
    public interface IXpfRichReport : IRichReport
    {
        IXpfRichReportTemplateItemsSource DefaultTemplatesSource { get; }

        IXpfRichReportTemplateItemsSource StoreTemplatesSource { get; }

        IRichReportTemplate DesignerTemplate { get; }

        Task NewTemplateAsync();

        Task CloneAsync(Guid? templateKey);

        Task EditTemplateAsync(Guid? templateKey);

        void EditTemplate(IRichReportTemplate template);

        void ShowTemplatesEditorDialog(string title = null);
    }
}
