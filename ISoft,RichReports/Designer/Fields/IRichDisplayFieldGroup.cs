using ISoft_RichReports.Fields;
using System.Drawing;

namespace ISoft_RichReports.Designer.Fields
{
    public interface IRichDisplayFieldGroup : IRichFieldGroup
    {
        Color BackgroundColor { get; }

        Color FontColor { get; }

        new IRichDisplayFieldGroup ParentGroup { get; }

        IRichDisplayFieldsCollection Fields { get; }
    }
}
