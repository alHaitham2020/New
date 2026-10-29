namespace ISoft.RichReports.Editor.Preview
{
    /// <summary>
    /// Factory for creating template servers
    /// </summary>
    public interface IRichTemplatePreviewServerFactory
    {
        IRichDocumentPreviewServer Create();
    }
}
