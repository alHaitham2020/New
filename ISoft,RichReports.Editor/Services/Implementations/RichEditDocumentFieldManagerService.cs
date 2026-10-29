using ISoft.RichReports.Editor.Fields.Codes;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Formatting;
using ISoft.RichReports.Editor.Fields.Options;
using ISoft.RichReports.Editor.Fields.Regions;
using ISoft.RichReports.Editor.Fields.Search;
using ISoft.RichReports.Editor.Fields.Switches;
using ISoft.RichReports.Editor.Services.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace ISoft.RichReports.Editor.Services.Implementations
{
    /// <summary>
    /// Comprehensive field management service for creating, scanning, updating,
    /// and formatting fields in RichEdit documents.
    /// </summary>
    /// <remarks>
    /// Thread-safety: Instance methods are not thread-safe. Create new instances per thread if needed.
    /// </remarks>
    public class RichEditDocumentFieldManagerService : IRichEditDocumentFieldManagerService
    {
        #region Dependencies
        private readonly IRichFieldCodeScanner _scanner;
        private readonly IRichFieldCodeBuilder _codeBuilder;
        private readonly IRichFieldSwitchBuilder _switchBuilder;
        #endregion

        #region Constructor
        public RichEditDocumentFieldManagerService(
            IRichFieldCodeScanner scanner = null,
            IRichFieldCodeBuilder builder = null,
            IRichFieldSwitchBuilder switchBuilder = null)
        {
            _scanner = scanner ?? new RichFieldCodeScanner();
            _switchBuilder = switchBuilder ?? new RichFieldSwitchBuilder();
            _codeBuilder = builder ?? new RichFieldCodeBuilder(new RichFieldInstructionsBuilder(_switchBuilder));
        }
        #endregion

        #region Field Creation

        public Field CreateMergeField(DocumentPosition position, string fieldPath, RichFieldFormat? format = null)
            => CreateField(position, _codeBuilder.CreateMergeField(fieldPath, format));

        public Field CreateMergeField(DocumentPosition position, RichMergeFieldOptions options)
            => CreateField(position, _codeBuilder.CreateMergeField(options));

        public Field CreatePictureField(DocumentPosition position, RichIncludePictureFieldOptions options)
            => CreateField(position, _codeBuilder.CreatePictureField(options));

        public Field CreatePictureField(DocumentPosition position, string uri)
            => CreateField(position, _codeBuilder.CreatePictureField(uri));

        public Field CreatePictureFieldFromImageUri(DocumentPosition position, ImageUri uri)
            => CreatePictureField(position, uri);

        public Field CreateNestedField(DocumentPosition position, RichNestedFieldOptions options)
        {
            ArgumentGuardHelper.ArgumentNotNull(position, nameof(position));
            ArgumentGuardHelper.ArgumentNotNull(options, nameof(options));
            ArgumentGuardHelper.ArgumentNotNull(options.OuterFieldOptions, nameof(options.OuterFieldOptions));
            ArgumentGuardHelper.ArgumentNotNull(options.InnerFieldOptions, nameof(options.InnerFieldOptions));

            // Resolve the owning subdocument ONCE so every piece (outer field, space,
            // inner field, switches) lands in the same piece table.
            SubDocument subDoc = position.BeginUpdateDocument();
            try
            {
                // 1. Outer shell field.
                Field outerField = CreateFieldCore(subDoc, position, options.OuterFieldOptions.FieldType);

                // 2. Space placeholder establishes the inner field boundary inside the outer code.
                var spaceRange = subDoc.InsertText(
                    subDoc.CreatePosition(outerField.CodeRange.End.ToInt()), " ");

                // 3. Inner field, inserted after the placeholder.
                CreateFieldWithOptions(spaceRange.End, options.InnerFieldOptions);

                // 4. Outer switches appended after the inner field.
                var switchesStr = _codeBuilder.GetFieldSwitchesBuildString(options.OuterFieldOptions);
                if (!string.IsNullOrEmpty(switchesStr))
                {
                    subDoc.InsertText(
                        subDoc.CreatePosition(outerField.CodeRange.End.ToInt()), switchesStr);
                }

                return outerField;
            }
            finally
            {
                position.EndUpdateDocument(subDoc);
            }
        }

        public Field CreateMergePictureField(DocumentPosition position,
            RichMergeFieldOptions mergeFieldOptions, RichIncludePictureFieldOptions pictureFieldOptions)
            => CreateNestedField(position,
                RichNestedFieldOptions.CreateIncludePictureWithInnerOptions(pictureFieldOptions, mergeFieldOptions));

        public Field CreateMergePictureField(DocumentPosition position, string mergeFieldPath)
            => CreateMergePictureField(
                position,
                RichMergeFieldOptions.Create(mergeFieldPath),
                RichIncludePictureFieldOptions.CreateDefault());

        public Field CreateTableStartField(DocumentPosition position, string tableName)
            => CreateField(position, _codeBuilder.CreateTableStart(tableName));

        public Field CreateTableEndField(DocumentPosition position, string tableName)
            => CreateField(position, _codeBuilder.CreateTableEnd(tableName));

        public Field CreateDateField(DocumentPosition position, RichDateBasedFieldOptions options)
            => CreateField(position, _codeBuilder.CreateDateField(options));

        public Field CreateDateField(DocumentPosition position, RichFieldFormat? format = null)
            => CreateField(position, _codeBuilder.CreateDateField(format));

        public Field CreateTimeField(DocumentPosition position, RichFieldFormat? format = null)
            => CreateField(position, _codeBuilder.CreateTimeField(format));

        public Field CreatePageField(DocumentPosition position)
            => CreateField(position, _codeBuilder.CreatePageField());

        public Field CreateNumPagesField(DocumentPosition position)
            => CreateField(position, _codeBuilder.CreateNumPagesField());

        public Field CreateIfField(DocumentPosition position,
            string expression1, string cmOperator, string expression2)
            => CreateField(position, _codeBuilder.CreateIfField(expression1, cmOperator, expression2));

        public Field CreateIfField(DocumentPosition position, RichIfFieldOptions options)
            => CreateField(position, _codeBuilder.CreateIfField(options));

        public Field CreateFieldInCell(TableCell cell, IRichFieldCode fieldCode, bool atStart)
        {
            ArgumentGuardHelper.ArgumentNotNull(cell, nameof(cell));

            var position = atStart ? cell.ContentRange.Start : cell.ContentRange.End;
            return CreateField(position, fieldCode);
        }

        public Field CreateFieldWithCommonOptions(DocumentPosition position,
            RichEditFieldType fieldType, string firstArgument, RichFieldFormat? format = null)
            => CreateField(position, _codeBuilder.CreateFieldWithCommonOptions(fieldType, firstArgument, format));

        public Field CreateFieldWithOptions(DocumentPosition position, object options)
            => options is RichNestedFieldOptions nested
                ? CreateNestedField(position, nested)
                : CreateField(position, _codeBuilder.CreateFieldWithOptions(options));

        protected Field CreateField(DocumentPosition position, RichEditFieldType fieldType)
            => CreateField(position, _codeBuilder.CreateField(fieldType));

        /// <summary>
        /// Core field insertion. Resolves the owning subdocument (body/header/footer/textbox)
        /// from the position, then creates the field in that context.
        /// </summary>
        protected Field CreateField(DocumentPosition position, IRichFieldCode fieldCode)
        {
            ArgumentGuardHelper.ArgumentNotNull(position, nameof(position));
            ArgumentGuardHelper.ArgumentNotNull(fieldCode, nameof(fieldCode));

            // Resolve the subdocument that actually owns this position.
            SubDocument subDoc = position.BeginUpdateDocument();
            try
            {
                return CreateFieldCore(subDoc, position, fieldCode);
            }
            finally
            {
                position.EndUpdateDocument(subDoc);
            }
        }

        private static Field CreateFieldCore(SubDocument subDoc, DocumentPosition position, IRichFieldCode fieldCode)
        {
            ArgumentGuardHelper.ArgumentNotNull(subDoc, nameof(subDoc));
            ArgumentGuardHelper.ArgumentNotNull(position, nameof(position));
            ArgumentGuardHelper.ArgumentNotNull(fieldCode, nameof(fieldCode));

            int posIndex = position.ToInt();
            if (posIndex < 0 || posIndex > subDoc.Range.End.ToInt())
                throw new InvalidOperationException(
                    $"Position index {posIndex} is outside the valid range of the subdocument.");

            return subDoc.Fields.Create(position, fieldCode.Build());
        }

        private Field CreateFieldCore(SubDocument subDoc, DocumentPosition position, RichEditFieldType fieldType)
            => CreateFieldCore(subDoc, position, _codeBuilder.CreateField(fieldType));

        #endregion

        #region Field Scanning and Detection

        public IRichFieldFindResultCollection FindMergeFields(Document document)
            => FindDocumentFields(document,
                code => code.FieldType == RichEditFieldType.MergeField && !code.IsRegion);

        public IRichFieldFindResultCollection FindDataTableMergeFields(Document document)
            => FindMergeFields(document).GetFiltredResult(f => f.FieldCode.IsDataTableField);

        public IRichFieldFindResultCollection FindTableStartFields(Document document)
            => FindDocumentFields(document, code => code.IsTableStart);

        public IRichFieldFindResultCollection FindTableEndFields(Document document)
            => FindDocumentFields(document, code => code.IsTableEnd);

        public IRichFieldFindResultCollection FindRowStartEndFields(TableRow row)
            => FindRowFields(row).GetFiltredResult(f => f.FieldCode.IsRegion);

        public IRichFieldFindResultCollection FindAllDocumentFields(Document document)
            => FindDocumentFields(document, RichDocumentSearchScope.All);

        public IRichFieldFindResultCollection FindDocumentFields(Document document, RichDocumentSearchScope scope)
            => FindDocumentFields(document, filter: null, scope);

        public IRichFieldFindResultCollection FindDocumentFields(
            Document document,
            Func<IRichFieldCode, bool> filter,
            RichDocumentSearchScope scope = RichDocumentSearchScope.All)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));

            var results = new RichFieldFindResultCollection();

            foreach (var field in document.GetFields(scope))
            {
                var code = ScanFieldCode(field.Field.CodeRange);
                if (code == null) continue;
                if (filter != null && !filter(code)) continue;

                results.Add(new RichFieldFindResult(code, field.Field));
            }

            return results;
        }

        public bool IsDocumentContainsField(
            Document document,
            Func<IRichFieldCode, bool> filter,
            RichDocumentSearchScope scope = RichDocumentSearchScope.All)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            ArgumentGuardHelper.ArgumentNotNull(filter, nameof(filter));

            foreach (var field in document.GetFields(scope))
            {
                var code = ScanFieldCode(field.Field.CodeRange);
                if (code != null && filter(code)) return true;
            }
            return false;
        }

        /// <summary>Scans fields whose range falls inside the given table.</summary>
        public IRichFieldFindResultCollection FindTableFields(Table table)
        {
            ArgumentGuardHelper.ArgumentNotNull(table, nameof(table));

            return FindFieldsAtRange(table.Range);
        }

        public IRichFieldFindResultCollection FindRowFields(TableRow row)
        {
            ArgumentGuardHelper.ArgumentNotNull(row, nameof(row));

            var results = new RichFieldFindResultCollection();

            foreach (TableCell cell in row.Cells)
            {
                foreach (var result in FindCellFields(cell))
                    results.Add(result);
            }

            return results;
        }

        public IRichFieldFindResultCollection FindCellFields(TableCell cell)
        {
            ArgumentGuardHelper.ArgumentNotNull(cell, nameof(cell));

            return FindFieldsAtRange(cell.Range);
        }

        public IRichFieldFindResultCollection FindFieldsAtRange(DocumentRange range)
        {
            ArgumentGuardHelper.ArgumentNotNull(range, nameof(range));

            var results = new RichFieldFindResultCollection();

            // Resolve the owning subdocument to safely enumerate fields in the range.
            SubDocument subDoc = range.BeginUpdateDocument();
            try
            {
                foreach (Field field in subDoc.Fields.Get(range))
                {
                    var code = ScanFieldCode(field.CodeRange);
                    if (code != null)
                        results.Add(new RichFieldFindResult(code, field));
                }
            }
            finally
            {
                range.EndUpdateDocument(subDoc);
            }

            return results;
        }

        public IRichFieldCode GetFieldCode(Field field)
        {
            return ScanFieldCode(field?.CodeRange);
        }

        public IRichFieldCode GetFieldCode(string rawCode) => ScanFieldCode(rawCode);

        public bool TableHasDifferentDataTables(Table table, string tableName)
        {
            if (table == null || string.IsNullOrEmpty(tableName)) return false;

            return FindTableFields(table).Any(r =>
                r.FieldCode != null
                && r.FieldCode.IsDataTableField
                && !r.FieldCode.GetDataTableName().EqualsTo(tableName));
        }

        public bool RowHasDifferentDataTables(TableRow row, string tableName)
        {
            if (row == null || string.IsNullOrEmpty(tableName)) return false;

            return FindRowFields(row).Any(r =>
                r.FieldCode != null
                && r.FieldCode.IsDataTableField
                && !r.FieldCode.GetDataTableName().EqualsTo(tableName));
        }

        public bool RowHasDataTableFields(TableRow row)
            => FindRowFields(row).Any(f => f.FieldCode.IsDataTableField);

        public string ExtractDataTableName(TableRow row)
        {
            if (row == null) return null;

            return FindRowFields(row)
                    .FirstOrDefault(f => f.FieldCode.IsDataTableField)
                    ?.FieldCode.GetDataTableName();
        }

        public bool RowHasTableRegions(Document document, TableRow row, string tableName)
        {
            if (document == null || row == null || string.IsNullOrEmpty(tableName)) return false;

            using var regionScanner = document.CreateRegionScanner();
            return regionScanner.IsRowHasRegion(row, tableName);
        }

        #endregion

        #region Field Update and Modification

        public void UpdateFieldArgument(Document document, Field field, int index, string newArg)
            => UpdateFieldCode(document, field, code => code.Arguments.AddOrUpdateValue(index, newArg));

        public void UpdateMergeFieldPath(Document document, Field field, string newPath)
            => UpdateFieldArgument(document, field, 0, newPath);

        public void UpdateFieldFormat(Document document, Field field, RichFieldFormat format)
        {
            if (RichFieldFormat.IsNullOrEmpty(format)) return;
            UpdateFieldCode(document, field, code => TryAddFieldFormatSwitch(code, format));
        }

        public void UpdateFieldFormat(Document document, Field field, RichFieldFormatType formatType, string formatValue)
        {
            if (string.IsNullOrWhiteSpace(formatValue) || formatType == RichFieldFormatType.None) return;

            UpdateFieldCode(document, field, code =>
            {
                var sw = _switchBuilder.CreateFormatSwitch(formatType, formatValue);
                if (sw != null) code.Switches.AddOrUpdate(sw);
            });
        }

        public void RemoveFieldFormat(Document document, Field field)
            => UpdateFieldCode(document, field, code =>
            {
                var toRemove = code.Switches.Where(s => s.IsFormatSwitch).ToList();
                foreach (var sw in toRemove) code.Switches.Remove(sw);
            });

        #endregion

        #region Field Removal

        public void RemoveSelectedField(Document document)
        {
            if (document == null) return;

            var selected = GetSelectedField(document);
            if (selected != null) RemoveField(document, selected);
        }

        public void RemoveField(Document document, Field field)
        {
            if (document == null || field == null) return;
            document.Delete(field.Range);
        }

        public void RemoveFieldsByType(Document document, RichEditFieldType fieldType)
        {
            if (document == null) return;

            // Materialize before deletion to avoid collection modification during iteration.
            var toRemove = FindDocumentFields(document, code => code.FieldType == fieldType)
                .Select(r => r.Field)
                .Where(f => f != null)
                .ToList();

            foreach (var field in toRemove)
                document.Delete(field.Range);
        }

        public void RemoveAllMergeFields(Document document)
            => RemoveFieldsByType(document, RichEditFieldType.MergeField);

        #endregion

        #region Formatting Helpers

        protected void AddNumericFormatSwitch(IRichFieldCode code, string value)
        {
            if (code == null || string.IsNullOrWhiteSpace(value)) return;
            code.Switches.AddOrUpdate(_switchBuilder.CreateNumericFormatSwitch(value));
        }

        protected void AddDateTimeFormatSwitch(IRichFieldCode code, string value)
        {
            if (code == null || string.IsNullOrWhiteSpace(value)) return;
            code.Switches.AddOrUpdate(_switchBuilder.CreateDateTimeFormatSwitch(value));
        }

        protected void AddGeneralFormatSwitch(IRichFieldCode code, string value)
        {
            if (code == null || string.IsNullOrWhiteSpace(value)) return;
            code.Switches.AddOrUpdate(_switchBuilder.CreateGeneralFormatSwitch(value));
        }

        protected void TryAddFieldFormatSwitch(IRichFieldCode code, RichFieldFormat? format)
        {
            if (code == null || RichFieldFormat.IsNullOrEmpty(format)) return;

            var sw = _switchBuilder.CreateFormatSwitch(format.Value);
            if (sw != null) code.Switches.AddOrUpdate(sw);
        }

        protected void RemoveExistingSwitch(IRichFieldCode code, RichSwitchType switchType)
        {
            if (code?.Switches == null) return;

            var existing = code.Switches.FirstOrDefault(s => s.SwitchType == switchType);
            if (existing != null) code.Switches.Remove(existing);
        }

        protected string GetFormat(IRichFieldCode code)
            => code?.Switches?.FirstOrDefault(s => s.IsFormatSwitch)?.SwitchValue;

        protected IRichFieldCode RemoveAllFormats(IRichFieldCode code)
        {
            if (code == null) return null;

            var copy = code.Copy();
            var formatSwitches = copy.Switches.Where(s => s.IsFormatSwitch).ToList();
            foreach (var sw in formatSwitches) copy.Switches.Remove(sw);
            return copy;
        }

        public string GetFieldFormat(Document document, Field field)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            return GetFormat(ScanFieldCode(field?.CodeRange));
        }

        public bool HasFormat(Document document, Field field, string format)
            => !string.IsNullOrEmpty(format)
               && string.Equals(GetFieldFormat(document, field), format, StringComparison.OrdinalIgnoreCase);

        #endregion

        #region Switches

        public void UpdateFieldSwitch(Document document, Field field, string switchKey, string switchValue)
        {
            if (string.IsNullOrWhiteSpace(switchKey)) return;

            UpdateFieldCode(document, field, code => code.Switches.AddOrUpdateSwitch(switchKey, switchValue));
        }

        public void UpdateSwitchValue(Document document, Field field, string switchKey, string newValue, bool addIfMissing = true)
            => UpdateFieldCode(document, field, code => UpdateSwitchValue(code, switchKey, newValue, addIfMissing));

        public void UpdateSwitchValue(IRichFieldCode fieldCode, string switchKey, string newValue, bool addIfMissing = true)
        {
            if (fieldCode == null || string.IsNullOrEmpty(switchKey)) return;

            if (fieldCode.Switches.ContainsKey(switchKey))
            {
                fieldCode.Switches.TryUpdateSwitch(switchKey, newValue);
            }
            else if (addIfMissing)
            {
                var sw = _switchBuilder.CreateFieldSwitch(switchKey, newValue);
                if (sw != null) fieldCode.Switches.AddOrUpdate(sw);
            }
        }

        public IRichFieldCode UpdateSwitchValueImmutable(IRichFieldCode fieldCode, string switchKey, string newValue)
        {
            if (fieldCode == null || string.IsNullOrEmpty(switchKey)) return fieldCode;

            var copy = fieldCode.Copy();
            UpdateSwitchValue(copy, switchKey, newValue, addIfMissing: true);
            return copy;
        }

        public void UpdateSwitches(Document document, Field field, Dictionary<string, string> switchUpdates)
        {
            if (switchUpdates == null || switchUpdates.Count == 0) return;

            UpdateFieldCode(document, field, code =>
            {
                foreach (var kv in switchUpdates)
                    UpdateSwitchValue(code, kv.Key, kv.Value, addIfMissing: true);
            });
        }

        public void UpdateSwitchInAllFields(Document document, RichEditFieldType fieldType, string switchKey, string newValue)
        {
            if (document == null) return;

            var targets = FindDocumentFields(document, code => code.FieldType == fieldType)
                .Select(r => r.Field)
                .Where(f => f != null)
                .ToList();

            foreach (var field in targets)
                UpdateSwitchValue(document, field, switchKey, newValue, addIfMissing: true);
        }

        public void UpdateMergeFieldsFormat(Document document, RichFieldFormatType formatType, string newFormat)
        {
            if (document == null) return;

            foreach (var result in FindMergeFields(document))
            {
                if (result.Field != null)
                    UpdateFieldFormat(document, result.Field, formatType, newFormat);
            }
        }

        #endregion

        #region Utility Methods

        public Field GetSelectedField(Document document)
        {
            if (document == null) return null;
            return document.GetSelectedField();
        }

        public bool IsValidFieldCode(string rawCode)
            => !string.IsNullOrWhiteSpace(rawCode) && GetFieldCode(rawCode) != null;

        public RichEditFieldType GetFieldType(string rawCode)
            => GetFieldCode(rawCode)?.FieldType ?? RichEditFieldType.Unknown;

        public RichEditFieldType GetFieldType(Field field)
           => GetFieldCode(field)?.FieldType ?? RichEditFieldType.Unknown;

        public List<string> GetAllMergeDataFieldPaths(Document document)
            => FindMergeFields(document)
                .Select(f => f.FieldCode?.FieldProperty)
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToList();

        public List<string> GetAllDataTablesName(Document document)
            => FindDataTableMergeFields(document)
                .Select(f => f.FieldCode?.GetDataTableName())
                .Where(p => !string.IsNullOrEmpty(p))
                .Distinct()
                .ToList();

        public void UpdateAllFields(Document document)
        {
            ArgumentGuardHelper.ArgumentNotNull(document, nameof(document));
            document.Fields.Update();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Single source of truth for scanning a field code from a range.
        /// Resolves the correct subdocument before reading text, preventing
        /// cross-document range errors.
        /// </summary>
        private IRichFieldCode ScanFieldCode(DocumentRange range)
        {
            if (range == null) return null;

            SubDocument subDoc = range.BeginUpdateDocument();
            try
            {
                string text = subDoc.GetText(range);
                return ScanFieldCode(text);
            }
            finally
            {
                range.EndUpdateDocument(subDoc);
            }
        }

        private IRichFieldCode ScanFieldCode(string rawCodeText)
        {
            if (string.IsNullOrWhiteSpace(rawCodeText)) return null;

            var code = _scanner.Scan(rawCodeText);
            return code != null && code.IsValid() ? code : null;
        }

        /// <summary>
        /// Common flow for any mutation of an existing field:
        ///   scan → mutate code → rebuild → replace in document.
        /// Wrapped in a single subdocument update so range operations stay consistent.
        /// </summary>
        private void UpdateFieldCode(Document document, Field field, Action<IRichFieldCode> mutate)
        {
            if (document == null || field == null || mutate == null) return;

            var code = ScanFieldCode(field.CodeRange);
            if (code == null) return;

            mutate(code);

            string newCode = code.Build();
            document.ReplaceFieldCode(field, newCode);
        }

        #endregion
    }
}
