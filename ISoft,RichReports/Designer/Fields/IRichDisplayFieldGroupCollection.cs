using System.Collections.Generic;

namespace ISoft.RichReports.Designer.Fields
{
    public interface IRichDisplayFieldGroupCollection : IRichDisplayFieldGroupReadOnlyCollection
    {
        internal void LoadFromSource(IEnumerable<IRichDisplayFieldGroup> source);

        internal void Clear();

        internal void InvalidateCache();
    }
}
