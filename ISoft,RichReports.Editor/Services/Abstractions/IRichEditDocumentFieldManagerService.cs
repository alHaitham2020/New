using ISoft.RichReports.Editor.Fields.Codes;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Formatting;
using ISoft.RichReports.Editor.Fields.Options;
using ISoft.RichReports.Editor.Fields.Search;
using System;
using System.Collections.Generic;
namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditDocumentFieldManagerService
    {
        Field CreateFieldWithCommonOptions(DocumentPosition position, RichEditFieldType fieldType, string firstArgument, RichFieldFormat? format = null);

        Field CreateDateField(DocumentPosition position, RichFieldFormat? format = null);

        Field CreateTimeField(DocumentPosition position, RichFieldFormat? format = null);

        Field CreateFieldInCell(TableCell cell, IRichFieldCode fieldCode, bool atStart);

        Field CreateIfField(DocumentPosition position, string expression, string trueText, string falseText);

        Field CreateMergeField(DocumentPosition position, string fieldPath, RichFieldFormat? format = null);

        Field CreateMergePictureField(DocumentPosition position, string fieldPath);

        Field CreateNumPagesField(DocumentPosition position);

        Field CreatePageField(DocumentPosition position);

        Field CreateTableEndField(DocumentPosition position, string tableName);

        Field CreateTableStartField(DocumentPosition position, string tableName);

        Field CreateFieldWithOptions(DocumentPosition position, object options);

        Field CreatePictureField(DocumentPosition position, string uri);

        Field CreatePictureField(DocumentPosition position, RichIncludePictureFieldOptions options);

        Field CreatePictureFieldFromImageUri(DocumentPosition position, ImageUri uri);

        Field CreateMergePictureField(DocumentPosition position, RichMergeFieldOptions mergeFieldOptions, RichIncludePictureFieldOptions pictureFieldOptions);

        Field CreateNestedField(DocumentPosition position, RichNestedFieldOptions options);

        Field CreateDateField(DocumentPosition position, RichDateBasedFieldOptions options);

        void UpdateFieldFormat(Document document, Field field, RichFieldFormatType formatType, string format);

        string ExtractDataTableName(TableRow row);

        IRichFieldFindResultCollection FindDataTableMergeFields(Document document);

        IRichFieldFindResultCollection FindMergeFields(Document document);

        IRichFieldFindResultCollection FindRowStartEndFields(TableRow row);

        IRichFieldFindResultCollection FindTableEndFields(Document document);

        IRichFieldFindResultCollection FindTableStartFields(Document document);

        List<string> GetAllDataTablesName(Document document);

        List<string> GetAllMergeDataFieldPaths(Document document);

        IRichFieldCode GetFieldCode(string fieldCode);

        string GetFieldFormat(Document document, Field field);

        RichEditFieldType GetFieldType(string fieldCode);

        bool RowHasDataTableFields(TableRow row);

        bool TableHasDifferentDataTables(Table table, string sourceTableName);

        bool HasFormat(Document document, Field field, string format);

        bool IsValidFieldCode(string fieldCode);

        void RemoveAllMergeFields(Document document);

        void RemoveField(Document document, Field field);

        void RemoveFieldFormat(Document document, Field field);

        void RemoveFieldsByType(Document document, RichEditFieldType fieldType);

        bool RowHasTableRegions(Document document, TableRow row, string collectionPath);

        IRichFieldFindResultCollection FindCellFields(TableCell cell);

        IRichFieldFindResultCollection FindAllDocumentFields(Document document);

        IRichFieldFindResultCollection FindRowFields(TableRow row);

        IRichFieldFindResultCollection FindTableFields(Table table);

        IRichFieldFindResultCollection FindFieldsAtRange(DocumentRange range);

        IRichFieldFindResultCollection FindDocumentFields(Document document, Func<IRichFieldCode, bool> filter, RichDocumentSearchScope scope = RichDocumentSearchScope.All);

        IRichFieldFindResultCollection FindDocumentFields(Document document, RichDocumentSearchScope scope);

        bool IsDocumentContainsField(Document document, Func<IRichFieldCode, bool> filter, RichDocumentSearchScope scope = RichDocumentSearchScope.All);

        bool RowHasDifferentDataTables(TableRow row, string sourceTableName);

        IRichFieldCode GetFieldCode(Field field);

        RichEditFieldType GetFieldType(Field field);

        void UpdateAllFields(Document document);

        void UpdateFieldFormat(Document document, Field field, RichFieldFormat format);

        void UpdateMergeFieldPath(Document document, Field field, string newPath);

        Field GetSelectedField(Document document);

        void RemoveSelectedField(Document document);

        void UpdateFieldArgument(Document document, Field field, int index, string newArg);

        void UpdateFieldSwitch(Document document, Field field, string switchCode, string switchValue);

        void UpdateSwitchValue(Document document, Field field, string switchCode, string newValue, bool addIfMissing = true);
    }
}
