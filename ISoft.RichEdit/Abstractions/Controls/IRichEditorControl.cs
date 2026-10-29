using System.Drawing;
using System.IO;

namespace ISoft.RichEdit.Abstractions.Controls
{
    public interface IRichEditorControl : IRichEditControl
    {
        bool Modified { get; set; }

        float DpiX { get; }

        float DpiY { get; }

        internal DevExpress.XtraRichEdit.Model.DocumentModel DocumentModel { get; }

        DocumentLayout DocumentLayout { get; }

        DocumentLayoutUnit LayoutUnit { get; }

        bool IsSelectionInMainDocument { get; }

        T GetLayoutElement<T>(DocumentPosition position) where T : RangedLayoutElement;

        bool CreateNewDocument(bool raiseDocumentClosing = false);

        bool LoadDocumentTemplate(Stream stream, DocumentFormat documentFormat);

        bool LoadDocumentTemplate(Stream stream);

        bool LoadDocumentTemplate(byte[] buffer);

        bool LoadDocumentTemplate(byte[] buffer, DocumentFormat documentFormat);

        byte[] SaveDocument(DocumentFormat documentFormat);

        void SaveDocument(Stream stream, DocumentFormat documentFormat);

        IRichEditDocumentServer CreateDocumentServer();

        void MailMerge(Document document);

        void MailMerge(MailMergeOptions options, Document targetDocument);

        void ExportToImage(Stream stream);

        void ExportToImage(Stream stream, DevExpress.XtraPrinting.ImageExportOptions imageExportOptions);

        byte[] ExportToImage();

        System.Drawing.Point PointToScreen(System.Drawing.Point clientPoint);

        DocumentPosition GetPositionFromPoint(PointF clientPoint);

        Rectangle GetBoundsFromPosition(DocumentPosition caretPosition);

        Rectangle GetLayoutLogicalBoundsFromPosition(DocumentPosition position);

        System.Drawing.Point GetPointFromPosition(DocumentPosition position, bool topBound);

        DocumentPosition GetDropPosition(System.Drawing.Point point);

        RichEditBrushBase CreateBrush(Color color);

        RichEditPenBase CreatePen(System.Drawing.Color color, int thickness);
    }
}
