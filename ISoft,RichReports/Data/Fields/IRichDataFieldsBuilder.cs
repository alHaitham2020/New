using System;

namespace ISoft.RichReports.Data.Fields
{
    public interface IRichDataFieldsBuilder
    {
        RichDataFieldGroup[] BuildFromObject(object source, params RichDataFieldGroup[] defaultGroups);

        RichDataFieldGroup[] BuildFromObject<T>(params RichDataFieldGroup[] defaultGroups);

        RichDataFieldGroup[] BuildFromType(Type sourceType, params RichDataFieldGroup[] defaultGroups);

        RichDataFieldsReflectionBuilder WithCurrentOptions(Action<IRichDataFieldsBuildOptions> configure);

        void ClearCache();
    }
}
