using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public class RichDataFieldsProvider : IRichDataFieldsProvider
    {
        private readonly IRichDataFieldGroupCollection _fields;

        IRichDataFieldGroupCollection IRichDataFieldsProvider.DataFields => _fields;

        public RichDataFieldsProvider(IRichDataFieldGroupCollection fields)
        {
            _fields = fields;
        }

        public static IRichDataFieldsProvider Create(IRichDataFieldGroupCollection fields)
        {
            return new RichDataFieldsProvider(fields);
        }

        public static RichDataFieldsProvider Create(IEnumerable<IRichDataFieldGroup> fields)
        {
            return new RichDataFieldsProvider(new RichDataFieldGroupCollection(fields));
        }

        public static IRichDataFieldsProvider Create(object dataSource)
        {
            var fields = RichDataFieldsReflectionBuilder.Default.BuildFromObject(dataSource);
            return Create(fields);
        }

        public static IRichDataFieldsProvider Create<T>()
        {
            var fields = RichDataFieldsReflectionBuilder.Default.BuildFromObject<T>();
            return Create(fields);
        }

        public static IRichDataFieldsProvider Create(Type objType)
        {
            var fields = RichDataFieldsReflectionBuilder.Default.BuildFromObject(objType);
            return Create(fields);
        }
    }
}
