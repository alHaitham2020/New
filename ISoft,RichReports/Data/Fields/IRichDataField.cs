using ISoft.RichReports.Fields;

namespace ISoft.RichReports.Data.Fields
{
    public interface IRichDataField : IRichFieldNode
    {
        new IRichDataFieldGroup ParentGroup { get; }

        bool AllowUseDefaultEmptyValue { get; }

        string FieldDataName { get; }
    }

}
