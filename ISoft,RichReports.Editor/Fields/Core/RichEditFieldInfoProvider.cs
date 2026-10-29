using DevExpress.XtraRichEdit.Fields;
using ISoft.RichReports.Editor.Internal.Fields;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Editor.Fields.Core
{ 
    public static class RichEditFieldInfoProvider
    {
        // Single source of truth: the list of all field definitions
        private static readonly RichEditFieldInfo[] AllFields = [

            RichEditFieldInfo.StringField(RichEditFieldType.MergeField, MergefieldField.FieldType, RichEditFieldCategory.DataFields,true),

            RichEditFieldInfo.BoolField(RichEditFieldType.IfField, IfField.FieldType, RichEditFieldCategory.EquationAndFormula,true),
            RichEditFieldInfo.StringField(RichEditFieldType.SymbolField, SymbolField.FieldType, RichEditFieldCategory.EquationAndFormula,true),
            RichEditFieldInfo.BoolField(RichEditFieldType.CheckboxField, CheckBoxField.FieldType, RichEditFieldCategory.EquationAndFormula),
            RichEditFieldInfo.StringField(RichEditFieldType.ExpEditCalculatedField, RichExpressionEditorCalculatedField.FieldType, RichEditFieldCategory.EquationAndFormula,true),

            RichEditFieldInfo.StringField(RichEditFieldType.IndexEntryField, IndexEntryField.FieldType, RichEditFieldCategory.IndexAndTables,true),
            RichEditFieldInfo.StringField(RichEditFieldType.TocField, TocField.FieldType, RichEditFieldCategory.IndexAndTables,true),
            RichEditFieldInfo.StringField(RichEditFieldType.TocEntryField, TocEntryField.FieldType, RichEditFieldCategory.IndexAndTables,true),

            RichEditFieldInfo.StringField(RichEditFieldType.HyperlinkField, HyperlinkField.FieldType, RichEditFieldCategory.LinksAndReferences,true),
            RichEditFieldInfo.StringField(RichEditFieldType.IncludePictureField, IncludepictureField.FieldType, RichEditFieldCategory.LinksAndReferences,true),
            RichEditFieldInfo.StringField(RichEditFieldType.RefField, RefField.FieldType, RichEditFieldCategory.LinksAndReferences,true),
            RichEditFieldInfo.StringField(RichEditFieldType.PageRefField, PageRefField.FieldType, RichEditFieldCategory.LinksAndReferences,true),
            RichEditFieldInfo.StringField(RichEditFieldType.StyleRefField, StyleRefField.FieldType, RichEditFieldCategory.LinksAndReferences,true),

            RichEditFieldInfo.IntField(RichEditFieldType.PageField, PageField.FieldType, RichEditFieldCategory.Numbering),
            RichEditFieldInfo.IntField(RichEditFieldType.NumPagesField, NumPagesField.FieldType, RichEditFieldCategory.Numbering),
            RichEditFieldInfo.IntField(RichEditFieldType.SeqField, SequenceField.FieldType, RichEditFieldCategory.Numbering),
            RichEditFieldInfo.IntField(RichEditFieldType.SectionPagesField, SectionPageField.FieldType, RichEditFieldCategory.DocumentProperties),

            RichEditFieldInfo.DateOnlyField(RichEditFieldType.DateField, DateField.FieldType, RichEditFieldCategory.DateAndTime),
            RichEditFieldInfo.TimeOnlyField(RichEditFieldType.TimeField, TimeField.FieldType, RichEditFieldCategory.DateAndTime),

            RichEditFieldInfo.StringField(RichEditFieldType.DocVariableField, DocVariableField.FieldType, RichEditFieldCategory.DocumentVariables,true),
            RichEditFieldInfo.StringField(RichEditFieldType.CustomPropertyField, RichEditCustomPropertyField.FieldType, RichEditFieldCategory.CustomProperties,true),

            RichEditFieldInfo.StringField(RichEditFieldType.DocPropertyField, DocumentPropertyField.FieldType, RichEditFieldCategory.DocumentProperties,true),
            RichEditFieldInfo.StringField(RichEditFieldType.InfoField, InfoField.FieldType, RichEditFieldCategory.DocumentProperties,true),
            RichEditFieldInfo.StringField(RichEditFieldType.AuthorField, AuthorField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.StringField(RichEditFieldType.CommentsField, CommentsField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.DateTimeField(RichEditFieldType.CreateDateField, CreatedateField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.DateTimeField(RichEditFieldType.SaveDateField, SaveDateField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.DateTimeField(RichEditFieldType.PrintDateField, PrintDateField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.StringField(RichEditFieldType.KeywordsField, KeywordsField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.StringField(RichEditFieldType.LastSavedByField, LastSavedByField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.IntField(RichEditFieldType.RevNumField, ReviewNumberField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.StringField(RichEditFieldType.SubjectField, SubjectField.FieldType, RichEditFieldCategory.DocumentProperties),
            RichEditFieldInfo.StringField(RichEditFieldType.TitleField, TitleField.FieldType, RichEditFieldCategory.DocumentProperties),
       ];

        // ---- Lookup caches (immutable after static ctor) ---------------------
        private static readonly FrozenDictionary<string, RichEditFieldInfo> FieldsByName;
        private static readonly FrozenDictionary<RichEditFieldType, RichEditFieldInfo> FieldsByType;
        private static readonly string[] AllFieldNames;

        static RichEditFieldInfoProvider()
        {
            // Allocation-free array transformation compared to Linq Select()
            AllFieldNames = Array.ConvertAll(AllFields, f => f.FieldName);

            FieldsByName = AllFields.ToFrozenDictionary(f => f.FieldName, StringComparer.OrdinalIgnoreCase);
            FieldsByType = AllFields.ToFrozenDictionary(f => f.FieldType);
        }

        // ---- Read-only accessors ---------------------------------------------
        public static IReadOnlyList<RichEditFieldInfo> GetAllFields() => AllFields;

        public static IReadOnlyList<string> GetAllFieldsName() => AllFieldNames;

        // ---- Name → info ------------------------------------------------------
        public static RichEditFieldInfo? GetFieldInfoByName(string fieldName)
        {
            if (string.IsNullOrWhiteSpace(fieldName)) return null;

            return FieldsByName.TryGetValue(fieldName.Trim(), out var fieldInfo) ? fieldInfo : null;
        }

        public static RichEditFieldInfo GetFieldInfoOrEmpty(string fieldName)
            => GetFieldInfoByName(fieldName) ?? RichEditFieldInfo.Unknown;

        // ---- Type → info ------------------------------------------------------
        public static RichEditFieldInfo? GetFieldInfoByType(RichEditFieldType fieldType)
        {
            if (fieldType == RichEditFieldType.Unknown) return null;

            return FieldsByType.TryGetValue(fieldType, out var fieldInfo) ? fieldInfo : null;
        }

        public static bool TryGetFieldInfoByType(RichEditFieldType fieldType,out RichEditFieldInfo fieldInfo)
        {
            if (fieldType == RichEditFieldType.Unknown)
            {
                fieldInfo = RichEditFieldInfo.Unknown;
                return false;
            }

            return FieldsByType.TryGetValue(fieldType, out fieldInfo);
        }

        public static bool TryGetFieldInfoByName(string fieldName, out RichEditFieldInfo fieldInfo)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                fieldInfo = RichEditFieldInfo.Unknown;
                return false;
            }

            return FieldsByName.TryGetValue(fieldName.Trim(), out fieldInfo);
        }

        public static RichEditFieldInfo GetFieldInfoByTypeOrEmpty(RichEditFieldType fieldType)
            => GetFieldInfoByType(fieldType) ?? RichEditFieldInfo.Unknown;

        public static RichEditFieldInfo GetRequiredFieldInfoByType(RichEditFieldType fieldType)
           => GetFieldInfoByType(fieldType) ?? throw new ArgumentNullException(nameof(fieldType));

        // ---- Type → name ------------------------------------------------------
        public static string GetFieldName(RichEditFieldType fieldType)
            => GetFieldInfoByType(fieldType)?.FieldName;

        public static string GetFieldNameOrEmpty(RichEditFieldType fieldType)
            => GetFieldInfoByTypeOrEmpty(fieldType).FieldName;

        /// <summary>
        /// Validates if a field type string is in the supported types collection.
        /// </summary>
        public static bool IsSupportedFieldType(string fieldType)
        {
            return !string.IsNullOrEmpty(fieldType) &&
                   AllFieldNames.Contains(fieldType);
        }

        /// <summary>
        /// Validates if a field type string is in the supported types collection.
        /// </summary>
        public static bool IsSupportedFieldType(RichEditFieldType fieldType)
        {
            return fieldType!= RichEditFieldType.Unknown &&
                   FieldsByType.ContainsKey(fieldType);
        }
    }
}
