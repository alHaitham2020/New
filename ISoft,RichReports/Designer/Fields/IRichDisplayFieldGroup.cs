using ISoft.RichReports.Fields;
using System.Drawing;

namespace ISoft.RichReports.Designer.Fields
{
    public interface IRichDisplayFieldGroup : IRichFieldGroup
    {
        Color BackgroundColor { get; }

        Color FontColor { get; }

        new IRichDisplayFieldGroup ParentGroup { get; }

        IRichDisplayFieldsCollection Fields { get; }
    }
}
