using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Instructions;
using ISoft.RichReports.Editor.Fields.Options;

namespace ISoft.RichReports.Editor.Fields.Codes
{
    public interface IRichFieldCodeBuilder
    {
        IRichFieldCode CreateFieldWithOptions(object options);

        IRichFieldCode CreateFieldWithOptions(object options, InstructionsBuilderTargets targets);

        IRichFieldCode CreateFieldWithCommonOptions(RichEditFieldType fieldType, string firstArguemnt, RichFieldFormat? format = null);

        IRichFieldCode CreateIfField(string expression1, string cmOperator, string expression2);

        IRichFieldCode CreateMergeField(string fieldPath, RichFieldFormat? format = null);

        IRichFieldCode CreateDocumentPropertyField(string propertyName, RichFieldFormat? format = null);

        IRichFieldCode CreateCustomPropertyField(string propertyName, RichFieldFormat? format = null);

        IRichFieldCode CreateDocumentVariableField(string variableName, RichFieldFormat? format = null);

        IRichFieldCode CreateTableEnd(string collectionPath);

        IRichFieldCode CreateTableStart(string collectionPath);

        IRichFieldCode CreateNumPagesField();

        IRichFieldCode CreatePageField();

        IRichFieldCode CreatePictureField(string uri);

        IRichFieldCode CreateMergeField(RichMergeFieldOptions options);

        IRichFieldCode CreateDocumentPropertyField(RichDocPropertyFieldOptions options);

        IRichFieldCode CreateCustomPropertyField(RichDocCustomPropertyFieldOptions options);

        IRichFieldCode CreateDocumentVariableField(RichDocVariableFieldOptions options);

        IRichFieldCode CreateIfField(RichIfFieldOptions options);

        IRichFieldCode CreateDateField(RichDateBasedFieldOptions options);

        IRichFieldCode CreateDateField(RichFieldFormat? format = null);

        IRichFieldCode CreateTimeField(RichFieldFormat? format = null);

        IRichFieldCode CreatePictureField(RichIncludePictureFieldOptions options);

        IRichFieldCode CreateField(RichEditFieldType fieldType);

        string GetFieldSwitchesBuildString(object options);

        string GetFieldArgumentsBuildString(object options);
    }
}
