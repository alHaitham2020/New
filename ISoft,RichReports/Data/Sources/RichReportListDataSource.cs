using ISoft.RichReports.Data.Fields;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace ISoft.RichReports.Data.Sources
{
    public class RichReportListDataSource : RichReportDataSourceBase
    {
        private readonly IRichDataFieldsBuilder _builder;

        public RichReportListDataSource(Type itemType, IList data)
            : this(itemType, data, new RichDataFieldsBuildOptions())
        {

        }

        public RichReportListDataSource(Type itemType, IList data, IRichDataFieldsBuildOptions options) 
            : this(itemType, data, new RichDataFieldsReflectionBuilder(options))
        {

        }

        public RichReportListDataSource(Type itemType, IList data, IRichDataFieldsBuilder builder) 
            : base(itemType, data)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public override RichDataFieldGroup[] GetDataFields()
        {
            return _builder.BuildFromType(SourceType);
        }

        public override async Task<RichDataFieldGroup[]> GetDataFieldsAsync()
        {
            return await Task.Run(GetDataFields);
        }
    }
}
