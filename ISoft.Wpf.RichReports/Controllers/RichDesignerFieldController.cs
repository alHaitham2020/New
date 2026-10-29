using DevExpress.XtraRichEdit.API.Native;
using ISoft.Mvvm.Helpers;
using ISoft.RichReportDesigner.Core.Constants;
using ISoft.RichReportDesigner.Core.Fields.Display;
using ISoft.RichReportDesigner.Core.RichEdit;
using ISoft.RichReportDesigner.Core.RichEdit.Services;
using ISoft.RichReportDesigner.UI.Controls;
using ISoft.Utils.Helpers;
using System;
using System.Drawing;

namespace ISoft.RichReportDesigner.UI.Controllers
{
    internal sealed class RichDesignerFieldController
    {
        private readonly IRichReportDesignerControl _designer;

        private Document Document => _designer.Editor.Document;

        private IRichFieldManagerService FieldManagerService => _designer.GetService<IRichFieldManagerService>();

        private bool HighlightingEnabled => _designer.Editor.FieldHighlightingEnabled;

        public RichDesignerFieldController(IRichReportDesignerControl designer)
        {
            _designer = designer ?? throw new ArgumentNullException(nameof(designer));
        }

        // ── Calculated-field callbacks (invoked by the designer) ───
        public void OnCalculatedFieldChanged(string fieldName)
        {
            _designer.Report.OnCalculatedFieldChanged(fieldName);

            var fieldProperty = _designer.Report.CalculatedFieldProperties.FindByName(fieldName);
            if (fieldProperty == null) return;

            var group = _designer.DisplayFields
                            .FindGroupByCategory(RichDisplayFieldGroupCategory.CalculatedFields)
                            as RichDisplayFieldGroup;

            if (group == null) return;

            group.AddCalculatedPropertyField(fieldProperty);

            var field = group.Fields.FindByFieldProperty(fieldName);
            if (field == null) return;

            UpdateDocumentField(field);
            _designer.DisplayFields.InvalidateCache();
        }

        public void OnCalculatedFieldRemoved(string fieldName)
        {
            _designer.Report.OnCalculatedFieldRemoved(fieldName);

            var group = _designer.DisplayFields
                            .FindGroupByCategory(RichDisplayFieldGroupCategory.CalculatedFields)
                            as RichDisplayFieldGroup;

            if (group == null) return;

            var field = group.Fields.FindByFieldProperty(fieldName);
            if (field == null) return;

            group.RemoveField(field);
            _designer.DisplayFields.InvalidateCache();
        }

        public bool CanRemoveCalculatedField(string fieldName)
        {
            var group = _designer.DisplayFields.FindGroupByCategory(RichDisplayFieldGroupCategory.CalculatedFields);
            if (group == null) return true;

            var field = group.Fields.FindByFieldProperty(fieldName);
            return field == null || !IsDocumentContainsField(field);
        }

        #region Public API — Formatting

        public void ClearHighlightsFields()
        {
            //if (HighlightingEnabled) return;

            Document.ExecuteOperation(doc =>
            {
                foreach (var result in FieldManagerService.FindAllDocumentFields(Document))
                {
                    var display = _designer.DisplayFields.FindFieldByKey(result.FieldCode.FieldKey);
                    if (display == null) continue;

                    ClearFieldHighlight(result.Field, display.BackgroundColor);
                }
            });
        }

        public void HighlightsFields()
        {
            if (!HighlightingEnabled) return;

            Document.ExecuteOperation(doc =>
            {
                foreach (var result in FieldManagerService.FindAllDocumentFields(doc))
                {
                    result.Field.Update();

                    var display = _designer.DisplayFields.FindFieldByKey(result.FieldCode.FieldKey);
                    if (display == null) continue;

                    SetFieldHighlight(result.Field, display.BackgroundColor);
                }
            });
        }

        public void RefreshHighlightsFields()
        {
            ClearHighlightsFields();
            HighlightsFields();
        }

        /// <summary>Returns true if the given field (by FieldKey) exists in the document.</summary>
        public bool IsDocumentContainsField(IRichDisplayField displayField)
            => displayField != null
               && FieldManagerService.IsDocumentContainsField(Document, c => c.FieldKey == displayField.FieldKey);

        /// <summary>Re-applies formatting for a single field across all its occurrences.</summary>
        public void UpdateDocumentField(IRichDisplayField displayField)
        {
            if (displayField == null) return;

            Document.ExecuteOperation(doc =>
            {
                var matches = FieldManagerService.FindDocumentFields(doc, c => c.FieldKey == displayField.FieldKey);

                foreach (var result in matches)
                {
                    result.Field.Update();
                    SetFieldHighlight(result.Field, displayField.BackgroundColor);
                }
            });
        }

        #endregion

        #region Public API — Insertion

        /// <summary>Inserts a field at the current caret position.</summary>
        public void InsertField(IRichDisplayField field)
            => InsertField(field, Document.CaretPosition);

        /// <summary>Inserts a field at an explicit document position (used by drag &amp; drop).</summary>
        public void InsertField(IRichDisplayField field, DocumentPosition position)
        {
            if (field == null) throw new ArgumentNullException(nameof(field));
            if (position == null) return;

            if (field.PreventMultipleInsertion && IsDocumentContainsField(field))
            {
                NotificationManagerHelper.ShowError(RichFieldDisplayErrors.FieldDuplicateInDocument);
                return;
            }

            if (field.IsDetailField)
                InsertDetailField(field, position);
            else
                InsertFieldAtPosition(position, field);
        }

        #endregion

        #region Private — Formatting helpers
        private void ClearHighlight(DocumentRange range, Color color)
        {
            if (range == null) return;

            range.DoWithSubDocument(d =>
            {
                var cp = d.BeginUpdateCharacters(range);
                try
                {
                    cp.HighlightColor = cp.HighlightColor == color ? d.PageBackColor : cp.BackColor;
                }
                finally
                {
                    d.EndUpdateCharacters(cp);
                }
            });
        }

        private void ClearFieldHighlight(Field field, Color color)
        {
            var parentOrField = field.GetTopLevelParent();
            ClearHighlight(parentOrField.ResultRange, color);
        }

        private void SetFieldHighlight(Field field, Color color)
        {
            var parentOrField = field.GetTopLevelParent();
            SetHighlight(parentOrField.ResultRange, color);
        }

        /// <summary>Applies a highlight color to a range (no-op if range or highlighting disabled).</summary>
        private void SetHighlight(DocumentRange range, Color color)
        {
            if (range == null || !HighlightingEnabled) return;

            range.DoWithSubDocument(d =>
            {
                var cp = d.BeginUpdateCharacters(range);
                try
                {
                    cp.HighlightColor = color;
                }
                finally
                {
                    d.EndUpdateCharacters(cp);
                }
            });
        }

        #endregion

        #region Private — Insertion core

        /// <summary>
        /// Detail (collection) fields must live inside a table cell, and all data
        /// fields sharing a row must use the same DataMember.
        /// </summary>
        private void InsertDetailField(IRichDisplayField field, DocumentPosition position)
        {
            var cell = position.WithSubDocument(d => d.Tables).GetTableCell(position);
            if (cell == null)
            {
                NotificationManagerHelper.ShowError(RichFieldDisplayErrors.FieldMustBeInTable);
                return;
            }

            if (FieldManagerService.RowHasDifferentDataTables(cell.Row, field.DataMember))
            {
                NotificationManagerHelper.ShowError(RichFieldDisplayErrors.TableRowDiffrentDataMembers);
                return;
            }

            InsertFieldAtPosition(position, field);
        }

        /// <summary>
        /// Inserts the field next to any existing field it overlaps, separating them
        /// with a space. Falls back to a plain insertion when no overlap is found.
        /// </summary>
        private void InsertFieldAtPosition(DocumentPosition position, IRichDisplayField displayField)
        {
            ArgumentGuardHelper.ArgumentNotNull(position, nameof(position));
            ArgumentGuardHelper.ArgumentNotNull(displayField, nameof(displayField));

            // 1. If the drop position lands inside an existing field, insert next to it.
            var existing = position.FindFieldContainsPosition();
            var target = existing == null
                ? position
                : ResolvePositionAroundExisting(existing, position);

            // must execute in block
            Document.ExecuteOperation(doc =>
            {
                // 2. Create the field.
                var field = CreateField(target, displayField)
                    ?? throw new InvalidOperationException(
                        $"Failed to create a document field of type {displayField.FieldType}.");

                // 3. Surround with spaces and refresh.
                var endRange = field.Range.WithSubDocument(d =>
                {
                    d.InsertSingleLineText(field.Range.Start, " ");
                    return d.InsertSingleLineText(field.Range.End, " ");
                });

                field.Update();
                SetFieldHighlight(field, displayField.BackgroundColor);

                doc.CaretPosition = endRange.End;
            });
        }

        /// <summary>
        /// When dropping onto an existing field, inserts a separating space on the
        /// side closest to the drop point and returns the new insertion position.
        /// </summary>
        private static DocumentPosition ResolvePositionAroundExisting(Field existing, DocumentPosition dropPosition)
        {
            int dropIndex = dropPosition.ToInt();
            int visibleStart = existing.ResultRange.Start.ToInt();
            int visibleMid = visibleStart + (existing.ResultRange.Length / 2);

            return dropIndex <= visibleMid ? existing.Range.Start : existing.Range.End;
        }

        private Field CreateField(DocumentPosition position, IRichDisplayField displayField)
            => FieldManagerService.CreateFieldWithOptions(position, displayField.CreateFieldOptions());

        #endregion
    }
}
