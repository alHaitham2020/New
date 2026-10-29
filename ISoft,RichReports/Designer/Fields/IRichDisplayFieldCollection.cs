using System.Collections.Generic;

namespace ISoft.RichReports.Designer.Fields
{
    public interface IRichDisplayFieldsCollection : IRichDisplayFieldsReadOnlyCollection
    {
        internal void LoadFrom(IEnumerable<IRichDisplayField> source);
    }

}
