using ISoft.RichReports.Data.Fields;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.Data.DataSources
{
    public abstract class RichReportDataSourceBase : IRichReportDataSource
    {
        private readonly Type _sourceType;
        private readonly object _data;

        public Type SourceType => _sourceType;

        public object Data => _data;

        protected RichReportDataSourceBase(Type sourceType, object data)
        {
            _sourceType = sourceType;
            _data = data;
        }

        public abstract RichDataFieldGroup[] GetDataFields();

        public abstract Task<RichDataFieldGroup[]> GetDataFieldsAsync();
    }
}
