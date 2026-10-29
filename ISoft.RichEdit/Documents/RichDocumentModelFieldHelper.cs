using System;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichEdit.Documents
{
    /// <summary>
    /// Encapsulates the low-level field mutations performed against a
    /// <see cref="DocumentModel"/>. All public mutators run inside a single
    /// history transaction + document update, so callers don't have to
    /// remember to do it themselves. Batch overloads exist so that N fields
    /// are changed as a single undoable step and the layout is recomputed once.
    /// </summary>
    public class RichDocumentModelFieldHelper
    {
        private readonly DocumentModel documentModel;

        public RichDocumentModelFieldHelper(DocumentModel documentModel)
        {
            this.documentModel = documentModel ?? throw new ArgumentNullException(nameof(documentModel));
        }

        private PieceTable ActivePieceTable => documentModel.ActivePieceTable;

        private FieldCollection Fields => ActivePieceTable.Fields;

        #region Single-field mutators (open a fresh history transaction each call)

        public void ReplaceFieldCode(Field field, string code) =>
            ExecuteInHistory(() => ReplaceFieldCodeCore(field, code));

        public void UpdateTextInFieldCode(Field field, DocumentLogPosition position, int length, string changedText) =>
            ExecuteInHistory(() => UpdateTextInFieldCodeCore(field, position, length, changedText));

        public void DeleteTextFromFieldCode(Field field, DocumentLogPosition position, int length) =>
            ExecuteInHistory(() => DeleteTextFromFieldCodeCore(field, position, length));

        public void ModifyFieldResult(Field field, string result) =>
            ExecuteInHistory(() => ModifyFieldResultCore(field, result));

        public void DeleteFieldCode(Field field) =>
            ExecuteInHistory(() => DeleteFieldCodeCore(field));

        // NOTE: the original version did NOT wrap InsertTextIntoField in a
        // history transaction. That made it inconsistent with everything else
        // here (and easy to misuse outside a larger undo block). Wrapped here
        // so behavior is uniform — callers who need to batch should use the
        // batch overload below or wrap in their own HistoryTransaction.
        public void InsertTextIntoField(Field field, DocumentLogPosition position, string text) =>
            ExecuteInHistory(() =>
                ToggleCodeView(field, codeView: true, () => ActivePieceTable.InsertText(position, text)));
        #endregion

        #region Batch mutators (all fields changed inside ONE history transaction)
        /// <summary>
        /// Replaces the code of every supplied field, atomically. The
        /// <paramref name="codeProvider"/> is invoked lazily per field; returning
        /// <c>null</c> from the provider skips that field.
        /// </summary>
        public void ReplaceFieldCodes(IEnumerable<Field> fields, Func<Field, string> codeProvider)
        {
            var list = MaterializeFields(fields);
            if (list.Count == 0 || codeProvider == null) return;

            ExecuteInHistory(() =>
            {
                foreach (var field in list)
                {
                    var code = codeProvider(field);
                    if (code != null)
                        ReplaceFieldCodeCore(field, code);
                }
            });
        }

        /// <summary>
        /// Replaces the result of every supplied field, atomically.
        /// </summary>
        public void ModifyFieldResults(IEnumerable<Field> fields, Func<Field, string> resultProvider)
        {
            var list = MaterializeFields(fields);
            if (list.Count == 0 || resultProvider == null) return;

            ExecuteInHistory(() =>
            {
                foreach (var field in list)
                {
                    var result = resultProvider(field);
                    if (result != null)
                        ModifyFieldResultCore(field, result);
                }
            });
        }

        /// <summary>
        /// Deletes the code of every supplied field, atomically. The fields are
        /// processed in reverse document order so that each subsequent
        /// <c>GetFullCodeInterval</c> lookup is resolved against a piece table
        /// whose earlier positions have not yet been invalidated.
        /// </summary>
        public void DeleteFieldCodes(IEnumerable<Field> fields)
        {
            var list = MaterializeFields(fields);
            if (list.Count == 0) return;

            // MaterializeFields already returns a private copy; sort it for safety.
            list.Sort((a, b) => b.FirstRunIndex.CompareTo(a.FirstRunIndex));

            ExecuteInHistory(() =>
            {
                foreach (var field in list)
                    DeleteFieldCodeCore(field);
            });
        }

        public void DeleteFieldCodesInRange(RunIndex firstRunIndex, RunIndex lastRunIndex)
        {
            var fields = GetFieldsInsideInterval(firstRunIndex, lastRunIndex);
            if (fields.Count == 0) return;

            // Single transaction, single layout pass — regardless of field count.
            DeleteFieldCodes(fields.Where(f => f.Parent == null));
        }

        /// <summary>
        /// Deletes the text of every supplied field's code (range-based variant).
        /// Deletions are performed from latest to earliest position so that the
        /// earlier intervals remain valid while the later ones are removed.
        /// </summary>
        public void DeleteTextFromFieldCodes(IEnumerable<(Field Field, DocumentLogPosition Position, int Length)> items)
        {
            if (items == null) return;
            var list = items.ToList();
            if (list.Count == 0) return;

            // DocumentLogPosition does not implement IComparable — use operators.
            // Sort descending so earlier positions stay valid during deletion.
            list.Sort((a, b) =>
            {
                if (a.Position > b.Position) return -1;
                if (a.Position < b.Position) return 1;
                return 0;
            });

            ExecuteInHistory(() =>
            {
                foreach (var (field, pos, len) in list)
                    DeleteTextFromFieldCodeCore(field, pos, len);
            });
        }
        #endregion

        #region Core (no history, no update — callers guarantee they're in one)

        private void ReplaceFieldCodeCore(Field field, string code)
        {
            var interval = field.GetCodeInterval(ActivePieceTable);
            UpdateTextInFieldCodeCore(field, interval.Start, interval.Length, code);
        }

        private void UpdateTextInFieldCodeCore(Field field, DocumentLogPosition position, int length, string changedText) =>
            ToggleCodeView(field, codeView: true, () => ActivePieceTable.ReplaceText(position, length, changedText));

        private void DeleteTextFromFieldCodeCore(Field field, DocumentLogPosition position, int length) =>
            ToggleCodeView(field, codeView: true, () => ActivePieceTable.DeleteContent(position, length, false));

        private void ModifyFieldResultCore(Field field, string result) =>
            ToggleCodeView(field, codeView: false, () =>
            {
                var interval = field.GetResultInterval(ActivePieceTable);
                ActivePieceTable.ReplaceText(interval.Start, interval.Length, result);
            });

        private void DeleteFieldCodeCore(Field field)
        {
            var interval = field.GetFullCodeInterval(ActivePieceTable);
            DeleteTextFromFieldCodeCore(field, interval.Start, interval.Length);
        }
        #endregion

        #region Code-view toggle + history wrapper

        /// <summary>
        /// Temporarily forces the field's <see cref="Field.IsCodeView"/> flag
        /// while <paramref name="action"/> runs, then restores it. Needed
        /// because the piece-table mutations above only touch what's currently
        /// displayed (code vs. result), and we always want the caller's flag
        /// state preserved.
        /// </summary>
        private void ToggleCodeView(Field field, bool codeView, Action action)
        {
            var original = field.IsCodeView;
            field.IsCodeView = codeView;
            try { action(); }
            finally { field.IsCodeView = original; }
        }

        /// <summary>
        /// Opens one undoable transaction and one document update, runs
        /// <paramref name="action"/>, then closes them. This is the only place
        /// that touches <see cref="DocumentModel.BeginUpdate"/> /
        /// <see cref="HistoryTransaction"/>, which means batch callers pay for
        /// them exactly once.
        /// </summary>
        private void ExecuteInHistory(Action action)
        {
            documentModel.BeginUpdate();
            try
            {
                using (new HistoryTransaction(documentModel.History))
                    action();
            }
            finally
            {
                documentModel.EndUpdate();
            }
        }
        #endregion

        #region Field lookup (unchanged behavior, tidied up)
        public Field FindFirstField(RunIndex runIndex)
        {
            int num = FindFieldIndexByRunIndex(runIndex);
            if (num < 0)
            {
                return null;
            }

            return ActivePieceTable.Fields[num].GetTopLevelParent();
        }

        public Field FindLastField(RunIndex runIndex)
        {
            int num = FindLastFieldIndex(runIndex);
            if (num < 0)
            {
                return null;
            }

            return ActivePieceTable.Fields[num].GetTopLevelParent();
        }

        public List<Field> GetInnerCodeFields(IList<Field> fields, Field parentField) =>
            ActivePieceTable.FieldUpdater.GetInnerCodeFields(fields, parentField);

        public List<Field> GetInnerResultFields(IList<Field> fields, Field parentField) =>
            ActivePieceTable.FieldUpdater.GetInnerResultFields(fields, parentField);

        public Field GetInnerCodeFieldOrField(Field field) =>
            field == null ? null : (GetInnerCodeFields(Fields, field).FirstOrDefault() ?? field);

        public Field GetInnerCodeField(Field field) =>
            field == null ? null : GetInnerCodeFields(Fields, field).FirstOrDefault();

        public bool HasInnerCodeFields(Field field) =>
            field != null && GetInnerCodeFields(Fields, field).Count != 0;

        /// <summary>
        /// Returns all top-level fields whose run range falls inside
        /// [<paramref name="firstRunIndex"/>, <paramref name="lastRunIndex"/>].
        /// The <see cref="Fields"/> collection is ordered by position, so the
        /// scan stops as soon as a field extends past the interval.
        /// </summary>
        public List<Field> GetFieldsInsideInterval(RunIndex firstRunIndex, RunIndex lastRunIndex)
        {
            var result = new List<Field>();
            int i = FindFieldIndexByRunIndex(firstRunIndex);
            if (i < 0) return result;

            for (int count = Fields.Count; i < count; i++)
            {
                var field = Fields[i];
                if (field.LastRunIndex > lastRunIndex)
                    break;

                if (field.FirstRunIndex >= firstRunIndex)
                    result.Add(field);
            }

            return result;
        }

        public int FindFieldIndexByRunIndex(RunIndex runIndex)
        {
            int count = Fields.Count;
            var predicate = new FieldRunIndexComparable(runIndex);
            int num = Algorithms.BinarySearchReverseOrder(Fields, predicate);
            if (num < 0)
            {
                num = ~num;
                if (num >= count) return ~num;
            }
            return num;
        }

        public int FindLastFieldIndex(RunIndex runIndex)
        {
            int count = Fields.Count;
            var predicate = new FieldRunIndexComparable(runIndex);
            int num = Algorithms.BinarySearchReverseOrder(Fields, predicate);
            if (num < 0)
            {
                num = ~num;
                if (num >= count) num = count;
                return num - 1;
            }
            return num;
        }

        public string GetOrginalFieldCode(Field field) => field.Code.GetText(ActivePieceTable);

        public string GetCleanCode(Field field) => field.GetCleanCodeText(ActivePieceTable);
        #endregion

        /// <summary>
        /// Materializes an <see cref="IEnumerable{Field}"/> into a private,
        /// deduplicated list, dropping nulls. Using a copy avoids multiple
        /// enumeration of a lazy sequence and protects against the caller
        /// mutating the source while we're iterating.
        /// </summary>
        private static List<Field> MaterializeFields(IEnumerable<Field> fields)
        {
            if (fields == null) return new List<Field>();

            var seen = new HashSet<Field>();
            var result = new List<Field>();
            foreach (var f in fields)
            {
                if (f != null && seen.Add(f))
                    result.Add(f);
            }
            return result;
        }
    }
}
