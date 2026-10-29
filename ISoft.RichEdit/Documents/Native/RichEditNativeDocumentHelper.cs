using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using ISoft.Utils.Helpers;
using DevExpress.XtraRichEdit.API.Native.Implementation;
using DevExpress.XtraRichEdit.API.Layout;

namespace ISoft.RichEdit.Native
{
    /// <summary>
    /// Extension helpers for <see cref="Document"/> that centralize safe
    /// subdocument resolution, field manipulation, and range operations.
    /// </summary>
    /// <remarks>
    /// Rule of thumb: any operation on a <see cref="DocumentPosition"/> or
    /// <see cref="DocumentRange"/> must be performed through the <see cref="SubDocument"/>
    /// that owns it. Use <see cref="WithSubDocument{T}"/> or
    /// <c>position.BeginUpdateDocument()</c> / <c>range.BeginUpdateDocument()</c>.
    /// </remarks>
    public static class RichEditNativeDocumentHelper
    {
        #region SubDocument resolution
        public static SubDocument GetActiveSubDocument(this Document document)
        {
            if (document is NativeDocument native)
                return native.ActiveSubDocument ?? document as SubDocument;

            return document;
        }

        public static SubDocument GetSubDocument(this DocumentPosition position)
        {
            return position.WithSubDocument(sudoc => sudoc);
        }

        /// <summary>
        /// Returns the <see cref="SubDocumentType"/> in the given position,
        /// </summary>
        public static SubDocumentType GetSubDocumentType(this DocumentPosition position)
        {
           return position.WithSubDocument(sudoc => sudoc.GetSubDocumentType());
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given position,
        /// invokes <paramref name="action"/> on it, and always releases the lock.
        /// </summary>
        public static T WithSubDocument<T>(this DocumentPosition position, Func<SubDocument, T> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(position, nameof(position));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            SubDocument subDoc = position.BeginUpdateDocument();
            try
            {
                return action(subDoc);
            }
            finally
            {
                position.EndUpdateDocument(subDoc);
            }
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given position,
        /// invokes <paramref name="action"/> on it, and always releases the lock.
        /// </summary>
        public static void DoWithSubDocument(this DocumentPosition position, Action<SubDocument> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(position, nameof(position));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            SubDocument subDoc = position.BeginUpdateDocument();
            try
            {
                action(subDoc);
            }
            finally
            {
                position.EndUpdateDocument(subDoc);
            }
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given range,
        /// invokes <paramref name="action"/> on it, and always releases the lock.
        /// </summary>
        public static T WithSubDocument<T>(this DocumentRange range, Func<SubDocument, T> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(range, nameof(range));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            SubDocument subDoc = range.BeginUpdateDocument();
            try
            {
                return action(subDoc);
            }
            finally
            {
                range.EndUpdateDocument(subDoc);
            }
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given range,
        /// invokes <paramref name="action"/> on it, and always releases the lock.
        /// </summary>
        public static T WithSubDocument<T>(this DocumentRange range, Func<DocumentRange, SubDocument, T> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(range, nameof(range));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            SubDocument subDoc = range.BeginUpdateDocument();
            try
            {
                return action(range, subDoc);
            }
            finally
            {
                range.EndUpdateDocument(subDoc);
            }
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given range,
        /// invokes <paramref name="action"/> on it, and always releases the lock.
        /// </summary>
        public static void DoWithSubDocument(this DocumentRange range, Action<SubDocument> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(range, nameof(range));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            SubDocument subDoc = range.BeginUpdateDocument();
            try
            {
                action(subDoc);
            }
            finally
            {
                range.EndUpdateDocument(subDoc);
            }
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given range,
        /// invokes <paramref name="action"/> on it, and always releases the lock.
        /// </summary>
        public static T WithSubDocument<T>(this DocumentLayout layout, RangedLayoutElement element, Func<SubDocument, T> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(layout, nameof(layout));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));
            ArgumentGuardHelper.ArgumentNotNull(element, nameof(element));

            SubDocument subDoc = layout.BeginUpdateDocument(element);
            try
            {
                return action(subDoc);
            }
            finally
            {
                layout.EndUpdateDocument(subDoc);
            }
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given range,
        /// invokes <paramref name="action"/> on it, and always releases the lock.
        /// </summary>
        public static void DoWithSubDocument(this DocumentLayout layout, RangedLayoutElement element, Action<SubDocument> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(layout, nameof(layout));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));
            ArgumentGuardHelper.ArgumentNotNull(element, nameof(element));

            SubDocument subDoc = layout.BeginUpdateDocument(element);
            try
            {
                action(subDoc);
            }
            finally
            {
                layout.EndUpdateDocument(subDoc);
            }
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given field's code range.
        /// </summary>
        public static T WithSubDocument<T>(this Field field, Func<SubDocument, T> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(field, nameof(field));
            return field.Range.WithSubDocument(action);
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given field's code range.
        /// </summary>
        public static T WithCodeSubDocument<T>(this Field field, Func<SubDocument, T> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(field, nameof(field));
            return field.CodeRange.WithSubDocument(action);
        }

        /// <summary>
        /// Resolves the <see cref="SubDocument"/> that owns the given field's full range.
        /// </summary>
        public static T WithFieldSubDocument<T>(this Field field, Func<SubDocument, T> action)
        {
            ArgumentGuardHelper.ArgumentNotNull(field, nameof(field));
            return field.Range.WithSubDocument(action);
        }

        #endregion

        #region Document-level operations

        /// <summary>
        /// Wraps a document mutation in <c>BeginUpdate</c> / <c>EndUpdate</c>.
        /// </summary>
        public static void ExecuteOperation<T>(this T document, Action<T> action) where T : SubDocument
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            document.BeginUpdate();
            try
            {
                action(document);
            }
            finally
            {
                document.EndUpdate();
            }
        }

        /// <summary>
        /// Wraps a document mutation in <c>BeginUpdate</c> / <c>EndUpdate</c>.
        /// </summary>
        public static TR ExecuteOperationWithResult<T,TR>(this T document, Func<T,TR> action) where T : SubDocument
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            document.BeginUpdate();
            try
            {
               return action(document);
            }
            finally
            {
                document.EndUpdate();
            }
        }
        #endregion

        #region Field code manipulation

        /// <summary>
        /// Replaces the code of the field containing the given range's start.
        /// </summary>
        public static void ReplaceFieldCode(this Document document, DocumentRange range, string newCode)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            ArgumentGuardHelper.ArgumentNotNull(range, nameof(range));

            var field = FindFieldContainsPosition(range.Start);
            if (field != null)
                document.ReplaceFieldCode(field, newCode);
        }

        /// <summary>
        /// Replaces a field's code, toggling code view as needed.
        /// </summary>
        public static void ReplaceFieldCode(this Document document, Field field, string newCode)
        {
            if (field == null) return;

            document.ExecuteOperation(_ =>
                field.ToggleCodeView(true, () =>
                    field.WithCodeSubDocument(subDoc =>
                    {
                        subDoc.Replace(field.CodeRange, newCode);
                        return true;
                    })));
        }

        public static void InsertTextInFieldCode(this Document document, Field field, string text)
        {
            if (field == null) return;
            document.InsertTextInFieldCode(field, field.CodeRange.End, text);
        }

        public static void InsertTextInFieldCode(this Document document, Field field, DocumentPosition position, string text)
        {
            if (field == null || position == null) return;

            document.ExecuteOperation(_ =>
                field.ToggleCodeView(true, () =>
                    position.WithSubDocument(subDoc =>
                    {
                        subDoc.InsertText(position, text);
                        return true;
                    })));
        }

        /// <summary>
        /// Toggles code view on the field for the duration of <paramref name="action"/>,
        /// restoring the original state afterwards.
        /// </summary>
        public static void ToggleCodeView(this Field field, bool codeView, Action action)
        {
            ArgumentGuardHelper.ArgumentNotNull(field, nameof(field));
            ArgumentGuardHelper.ArgumentNotNull(action, nameof(action));

            bool original = field.ShowCodes;
            field.ShowCodes = codeView;
            try
            {
                action();
            }
            finally
            {
                field.ShowCodes = original;
            }
        }

        #endregion

        #region Field lookup

        /// <summary>
        /// Finds the field whose range contains <paramref name="position"/>.
        /// Uses the position's owning subdocument (not the active one).
        /// </summary>
        public static Field FindFieldContainsPosition(this DocumentPosition position)
        {
            if (position == null) return null;

            return position.GetSubDocumentFields().FirstOrDefault(f => f.Range.Contains(position));
        }

        /// <summary>
        /// Returns the field at the current selection, or null.
        /// </summary>
        public static Field GetSelectedField(this Document document)
            => document?.Selection is { } selection
                ? FindFieldContainsPosition(selection.Start)
                : null;

        public static ReadOnlyFieldCollection GetSubDocumentFields(this DocumentPosition position)
        {
            if (position == null) return null;

            return position.WithSubDocument(subDoc => subDoc.Fields);
        }

        /// <summary>
        /// Returens a top parent or current field if no parent found
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public static Field GetTopLevelParent(this Field field)
        {
            Field currentField = field;
            while (currentField.Parent != null)
            {
                currentField = currentField.Parent;
            }

            return currentField;
        }

        /// <summary>Fields nested inside the parent field's code.</summary>
        public static List<Field> GetInnerCodeFields(this Field parentField, ReadOnlyFieldCollection fields)
        {
            int count = fields.Count;
            List<Field> list = new List<Field>();
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Parent == parentField && fields[i].ResultRange.End < parentField.CodeRange.End)
                {
                    list.Add(fields[i]);
                }
            }

            return list;
        }

        /// <summary>Fields nested inside the parent field's result.</summary>
        public static List<Field> GetInnerResultFields(this Field parentField, ReadOnlyFieldCollection fields)
        {
            int count = fields.Count;
            List<Field> list = new List<Field>();
            for (int i = 0; i < count; i++)
            {
                if (fields[i].Parent == parentField && fields[i].CodeRange.Start >= parentField.ResultRange.Start)
                {
                    list.Add(fields[i]);
                }
            }

            return list;
        }
        #endregion

        #region Field deletion

        public static void DeleteField(this Document document, Field field)
        {
            if (document == null || field == null) return;

            document.ExecuteOperationWithResult(_ =>
                field.WithFieldSubDocument(subDoc =>
                {
                    subDoc.Delete(field.Range);
                    return true;
                }));
        }

        public static void DeleteFields(this Document document, Field[] fields)
        {
            if (document == null || fields == null || fields.Length == 0) return;

            document.ExecuteOperation(_ =>
            {
                foreach (var field in fields)
                {
                    if (field == null) continue;

                    // Each field may live in a different subdocument (body/header/footer).
                    field.WithFieldSubDocument(subDoc =>
                    {
                        subDoc.Delete(field.Range);
                        return true;
                    });
                }
            });
        }

        #endregion

        #region Text insertion

        /// <summary>
        /// Inserts text at the position using the position's owning subdocument.
        /// </summary>
        public static DocumentRange InsertText(this DocumentPosition position, string text)
        {
            if (position == null) return null;
            return position.WithSubDocument(subDoc => subDoc.InsertText(position, text));
        }

        #endregion

        #region Range / position conversion

        public static DocumentRange GetRangeFromFixed(this SubDocument document, FixedRange fixedRange)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            return document.CreateRange(fixedRange.Start, fixedRange.Length);
        }

        /// <summary>
        /// Returns the position of the last character of the fixed range
        /// (inclusive-end semantics: Start + Length - 1).
        /// </summary>
        public static DocumentPosition GetPositionFromFixedEnd(this SubDocument document, FixedRange fixedRange)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            return document.CreatePosition(fixedRange.Start + fixedRange.Length - 1);
        }

        public static DocumentPosition GetPositionFromFixedStart(this SubDocument document, FixedRange fixedRange)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            return document.CreatePosition(fixedRange.Start);
        }

        public static DocumentPosition GetPositionFromFixedRange(this SubDocument document, FixedRange fixedRange , bool? isRtl)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));

            int pos = isRtl.HasValue && isRtl.Value ? fixedRange.Start : fixedRange.Start + fixedRange.Length - 1; // must with -1
            return document.CreatePosition(pos);
        }

        public static FixedRange ToFixedRange(this DocumentRange range)
        {
            ArgumentGuardHelper.ArgumentNotNull(range, nameof(range));

            var frezed = range.GetAsFrozen();
            int start = frezed.Start.ToInt();
            int end = frezed.End.ToInt();
            return new FixedRange(start, end - start);
        }

        /// <summary>Half-open interval check: <c>Start &lt;= index &lt; End</c>.</summary>
        public static bool IsInRange(this DocumentRange range, int index)
            => range != null && index >= range.Start.ToInt() && index < range.End.ToInt();

        #endregion

        #region discovery

        public static void ProcessSubDocuments(this Document document, Action<SubDocument> action, RichDocumentSearchScope scope) 
            => RichDocumentDiscovery.ProcessSubDocuments(document, action, scope);

        public static void ProcessAllSubDocuments(this Document document, Action<SubDocument> action)
           => document.ProcessSubDocuments(action, RichDocumentSearchScope.All);

        public static IRichFieldCollection GetFields(this Document document, RichDocumentSearchScope scope)
            => RichDocumentDiscovery.GetFields(document, scope);

        /// <summary>
        /// Get all fields in all document sections
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public static IRichFieldCollection GetAllFields(this Document document)
            => document.GetFields(RichDocumentSearchScope.All);

        #endregion
    }
}
