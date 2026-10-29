using ISoft.RichReports.Fields;

namespace ISoft.RichReports.Data.Fields
{
    public interface IRichDataFieldGroup : IRichFieldGroup
    {
        new IRichDataFieldGroup ParentGroup { get; }

        IRichDataFieldCollection Fields { get; }
    }
}
