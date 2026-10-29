using DevExpress.XtraRichEdit.API.Native;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditDocumentCustomPropertiesManagerService
    {
        string GetDisplayName(string propertyName);

        void UpdateDocCustomProperties(DocumentCustomProperties properties);
    }
}
