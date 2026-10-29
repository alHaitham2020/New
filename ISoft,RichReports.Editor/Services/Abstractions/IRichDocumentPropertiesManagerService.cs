using DevExpress.XtraRichEdit.API.Native;
using ISoft.RichReports.Core.Templates;
using ISoft.RichReports.Editor.RichProperties;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditDocumentPropertiesManagerService
    {
        string GetDisplayName(RichDocumentPropertyId id);

        string GetDisplayName(string name);

        void UpdateDocDesignProperties(DocumentProperties properties, IRichReportTemplate template);

        void UpdateDocPreviewProperties(DocumentProperties properties, IRichReportTemplate template);
    }
}
