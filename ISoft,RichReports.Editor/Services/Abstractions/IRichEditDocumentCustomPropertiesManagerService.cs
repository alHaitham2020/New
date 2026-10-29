namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditDocumentCustomPropertiesManagerService
    {
        string GetDisplayName(string propertyName);

        void UpdateDocCustomProperties(DocumentCustomProperties properties);
    }
}
