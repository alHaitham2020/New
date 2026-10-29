using ISoft.RichReports.Designer.Fields;
using ISoft.RichReports.Models.Reports;

namespace ISoft.RichReports.Designer
{
    public interface IRichReportDesignerDisplayFieldsBuilder
    {
        IRichDisplayFieldGroup[] Build(IRichReport report);
    }
}
