using ISoft.RichReports.Fields;

namespace ISoft.RichReports.Designer.Fields
{
    public interface IRichDisplayField : IRichFieldNode, IRichFieldHighlightInfo
    {
        new IRichDisplayFieldGroup ParentGroup { get; }

        RichEditFieldType FieldType { get; }

        RichEditFieldKey FieldKey { get; }

        bool IsNestedPictureField { get; }

        bool PreventMultipleInsertion { get; }

        object CreateFieldOptions();
    }
}
