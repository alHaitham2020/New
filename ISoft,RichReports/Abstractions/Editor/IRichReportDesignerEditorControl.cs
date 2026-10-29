using DevExpress.XtraRichEdit.API.Native;
using ISoft.RichEdit.UI;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.UI
{
    public interface IRichReportDesignerEditorControl : IRichEditorControl
    {
        bool FieldHighlightingEnabled { get; }

        Task ShowPreviewAsync();

        bool CanShowPreview();

        Task SaveAsync();

        bool CanSave();

        void OnFieldHighlightingEnabledChanged();
    }

    public interface IRichReportDesignerEditorSurface
    {
        bool HasChanged { get; }

        Document Document { get; }

        bool LoadDocumentOrCreateNew(byte[] content);

        byte[] GetDocumentThumbnail();

        byte[] Save();

        void UpdateDocumentState(bool change);

        DocumentPosition GetDropPosition(System.Drawing.Point point);

        bool Focus();
    }
}
