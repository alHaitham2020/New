using ISoft.RichReports.Data.Fields;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.Data.Sources
{
    public interface IRichReportDataSource
    {
        object Data { get; }

        Type SourceType { get; }

        RichDataFieldGroup[] GetDataFields();

        Task<RichDataFieldGroup[]> GetDataFieldsAsync();
    }
}
