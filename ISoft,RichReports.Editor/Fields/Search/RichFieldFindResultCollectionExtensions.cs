using System;
using System.Linq;

namespace ISoft_RichReports.Editor.Fields.Search
{
    public static class RichFieldFindResultCollectionExtensions
    {
        public static IRichFieldFindResultCollection GetFiltredResult(this IRichFieldFindResultCollection result, Func<IRichFieldFindResult, bool> predicate)
        {
            return new RichFieldFindResultCollection(result.Where(predicate));
        }
    }
}
