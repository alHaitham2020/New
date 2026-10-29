using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public interface IRichDataFieldCollection : IRichDataFieldReadOnlyCollection
    {
        internal void LoadFrom(IEnumerable<IRichDataField> source);
    }

}
