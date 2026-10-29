using System.Collections.Generic;

namespace ISoft.RichReports.Designer.Fields
{
    public static class RichDisplayFieldGroupExtensions
    {
        public static IRichDisplayFieldsReadOnlyCollection ToFieldReadOnlyCollection(this IEnumerable<IRichDisplayField> source)
        {
            if (source == null) return null;

            return new RichDisplayFieldCollection(source);
        }

        public static IRichDisplayFieldGroupReadOnlyCollection ToGroupReadOnlyCollection(this IEnumerable<IRichDisplayFieldGroup> source)
        {
            if (source == null) return null;

            return new RichDisplayFieldGroupCollection(source);
        }
    }
}
