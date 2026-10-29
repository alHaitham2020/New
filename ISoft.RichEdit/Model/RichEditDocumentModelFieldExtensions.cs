using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using System;
using System.Text;

namespace ISoft.RichEdit.Model
{
    public static class RichEditDocumentModelFieldExtensions
    {
        public static DocumentModelPosition GetCodeStartPosition(this Field field, PieceTable table)
        {
            return DocumentModelPosition.FromRunEnd(table, field.Code.Start);
        }

        public static DocumentModelPosition GetCodeEndPosition(this Field field, PieceTable table)
        {
            return DocumentModelPosition.FromRunStart(table, field.Code.End);
        }

        /// <summary>
        /// returns clean code without start and end {}
        /// </summary>
        /// <param name="field"></param>
        /// <param name="pieceTable"></param>
        /// <returns></returns>
        public static string GetCleanCodeText(this Field field, PieceTable pieceTable)
        {
            StringBuilder stringBuilder = new();
            var start = field.Code.Start + 1;
            var end = field.Code.End - 1;
            for (RunIndex runIndex = start; runIndex <= end; ++runIndex)
            {
                stringBuilder.Append(pieceTable.GetRunPlainText(runIndex));
            }

            return stringBuilder.ToString();
        }

        public static DocumentLogInterval GetFullCodeInterval(this Field field, PieceTable table)
        {
            DocumentModelPosition start = DocumentModelPosition.FromRunStart(table, field.FirstRunIndex);
            DocumentModelPosition end = DocumentModelPosition.FromRunStart(table, field.LastRunIndex + 1);
            int leng = Math.Abs(end.LogPosition - start.LogPosition);
            return start.LogPosition.ToLogInterval(leng);
        }

        public static DocumentLogInterval GetCodeInterval(this Field field, PieceTable table)
        {
            var start = DocumentModelPosition.FromRunEnd(table, field.Code.Start);
            var end = DocumentModelPosition.FromRunStart(table, field.Code.End);
            var leng = end.LogPosition - start.LogPosition;
            return start.LogPosition.ToLogInterval(leng);
        }

        public static DocumentLogInterval GetCleanCodeInterval(this Field field, PieceTable table)
        {
            var start = DocumentModelPosition.FromRunEnd(table, field.Code.Start + 1);
            var end = DocumentModelPosition.FromRunStart(table, field.Code.End - 1);
            var leng = end.LogPosition - start.LogPosition;
            return start.LogPosition.ToLogInterval(leng);
        }

        public static DocumentLogInterval GetResultInterval(this Field field, PieceTable table)
        {
            var start = DocumentModelPosition.FromRunStart(table, field.Result.Start);
            var end = DocumentModelPosition.FromRunStart(table, field.Result.End);
            var leng = end.LogPosition - start.LogPosition;
            return start.LogPosition.ToLogInterval(leng);
        }

        public static T Parse<T>(this Field field) where T : CalculatedFieldBase
        {
            RichEditDocumentModelFieldParser parser = new();
            return parser.ParseField<T>(field);
        }

        public static bool TryParse<T>(this Field field, out T result) where T : CalculatedFieldBase
        {
            RichEditDocumentModelFieldParser parser = new();
            return parser.TryParseField(field, out result);
        }

        public static Field FindFieldBySelection(Selection selection)
        {
            FieldController fieldController = new FieldController();
            return fieldController.FindFieldBySelection(selection);
        }

        public static Field FindTopmostFieldBySelection(Selection selection)
        {
            FieldController fieldController = new FieldController();
            return fieldController.FindTopmostFieldBySelection(selection);
        }
    }
}
