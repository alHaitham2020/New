using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System;

namespace ISoft.RichEdit.Model
{
    public static class RichEditDocumentModelExtensions
    {
        public static DocumentLogInterval ToLogInterval(this DocumentLogPosition position, int length)
        {
            return new DocumentLogInterval(position, length);
        }

        public static DocumentModelPosition ToDocumentModelPosition(this DocumentLogPosition logPosition, PieceTable table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            // ToDocumentModelPosition requires an *inclusive* position within
            // [DocumentStartLogPosition, DocumentEndLogPosition]. Clamp so a
            // stale / boundary position never blows up the command's UI-state pass.
            var clamped = ClampToDocument(logPosition, table);
            return PositionConverter.ToDocumentModelPosition(table, clamped);
        }

        public static RunInterval GetRunInterval(this Selection selection, PieceTable table)
        {
            if (selection == null || table == null) return null;

            // Selection.NormalizedStart is inclusive.
            // Selection.NormalizedEnd is *exclusive* (== NormalizedVirtualEnd + 1
            // when Length > 0). Convert to inclusive before hitting the converter
            // — same rule Selection.NormalizedVirtualEnd uses internally.
            DocumentLogPosition startLog = selection.NormalizedStart;
            DocumentLogPosition endLog = selection.Length > 0
                ? selection.NormalizedEnd - 1
                : selection.NormalizedEnd;

            var startPos = startLog.ToDocumentModelPosition(table);
            var endPos = endLog.ToDocumentModelPosition(table);

            return new RunInterval(startPos.RunIndex, endPos.RunIndex);
        }

        private static DocumentLogPosition ClampToDocument(DocumentLogPosition pos, PieceTable table)
        {
            if (pos < table.DocumentStartLogPosition) return table.DocumentStartLogPosition;
            if (pos > table.DocumentEndLogPosition) return table.DocumentEndLogPosition;
            return pos;
        }
    }
}
