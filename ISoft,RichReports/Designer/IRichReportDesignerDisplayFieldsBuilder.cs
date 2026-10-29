using ISoft.RichReports.Designer.Fields;
using ISoft.RichReports.UI;

namespace ISoft.RichReports.Designer
{
    public interface IRichReportDesignerDisplayFieldsBuilder
    {
        IRichDisplayFieldGroup[] Build(IRichReport report);
    }
}
