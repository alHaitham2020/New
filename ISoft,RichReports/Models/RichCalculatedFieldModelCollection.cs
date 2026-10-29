using ISoft.RichReports.Core.Fields;
using ISoft.Utils.Data.Collection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.UI.DataModel
{
    public class RichCalculatedFieldModelCollection : ObservableUniqueDataCollectionBase<string, RichReportCalculatedFieldModel>
    {
        private static readonly Func<RichReportCalculatedFieldModel, string> _keySelector = (x) => x.FieldName;

        public RichCalculatedFieldModelCollection() : base(_keySelector)
        {

        }

        public RichCalculatedFieldModelCollection(IEnumerable<RichReportCalculatedFieldModel> collection) : base(collection, _keySelector)
        {

        }

        protected override bool IsValidKey(string key) => !string.IsNullOrWhiteSpace(key);

        protected override void InsertItem(int index, RichReportCalculatedFieldModel item)
        {
            if (string.IsNullOrEmpty(item?.FieldName))
            {
                item.FieldName = GenerateFieldName(this);
            }

            if (string.IsNullOrEmpty(item?.DisplayName))
            {
                item.DisplayName = GenerateFieldDisplayName(this);
            }

            if (!ContainsKey(item.FieldName))
            {
                base.InsertItem(index, item);
            }
        }

        internal string GenerateNewFieldName()
        {
            return GenerateFieldName(this);
        }

        internal string GenerateNewFieldDisplayName()
        {
            return GenerateFieldDisplayName(this);
        }

        internal static string GenerateFieldName(RichCalculatedFieldModelCollection fields)
        {
            int index = 1;
            while (fields.Any((RichReportCalculatedFieldModel w) => w.FieldName == $"{RichReportCalculatedFieldModel.FieldNamePrefix}{index}"))
            {
                index++;
            }

            return $"{RichReportCalculatedFieldModel.FieldNamePrefix}{index}";
        }

        internal static string GenerateFieldDisplayName(RichCalculatedFieldModelCollection fields)
        {
            int index = 1;
            while (fields.Any((RichReportCalculatedFieldModel w) => w.DisplayName == $"{RichReportCalculatedFieldModel.DisplayNamePrefix}{index}"))
            {
                index++;
            }

            return $"{RichReportCalculatedFieldModel.DisplayNamePrefix}{index}";
        }

        internal RichReportCalculatedFieldModel CreateNew(int? reportId)
        {
            var item = new RichReportCalculatedFieldModel()
            {
                Id = Guid.NewGuid(),
                ReportId = reportId,
                FieldType = RichCalculatedFieldType.String,
                FieldName = GenerateNewFieldName(),
                DisplayName = GenerateNewFieldDisplayName(),
                Expression = string.Empty,
                DataMember = string.Empty,
            };

            return item;
        }
    }
}
