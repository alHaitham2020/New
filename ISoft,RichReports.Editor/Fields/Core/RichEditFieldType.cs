namespace ISoft.RichReports.Editor.Fields.Core
{
    /// <summary>
    /// Represents the type of field in the document
    /// </summary>
    public enum RichEditFieldType
    {
        MergeField,

        DateField,
        TimeField,

        PageField,
        NumPagesField,
        SeqField,
        SectionPagesField,

        IfField,
        SymbolField,
        CheckboxField,

        IndexEntryField,
        TocField,
        TocEntryField,

        HyperlinkField,
        IncludePictureField,
        RefField,
        PageRefField,
        StyleRefField,

        CustomPropertyField,
        DocVariableField,
        ExpEditCalculatedField,

        DocPropertyField,
        InfoField,
        AuthorField,
        CommentsField,
        CreateDateField,
        KeywordsField,
        LastSavedByField,
        PrintDateField,
        RevNumField,
        SaveDateField,
        SubjectField,
        TitleField,

        Unknown
    }
}
