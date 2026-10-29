using ISoft.RichEdit.Documents;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichEdit.Documents
{
    public static class RichEditLayoutPageExtensions
    {
        public static IEnumerable<FixedRange> GetVisibleRanges(this LayoutPage layoutPage, IEnumerable<FixedRange> ranges)
        {
            if (ranges is null)
                return null;
            return ranges.Where(layoutPage.IsRangeVisible);
        }

        public static bool IsRangeVisible(this LayoutPage layoutPage, FixedRange range)
        {
            FixedRange visibleRange = layoutPage.MainContentRange;
            return visibleRange.Contains(range);
        }

       public static List<FixedRange> GetVisibleRanges(List<FixedRange> ranges, FixedRange visibleRange)
        {
            List<FixedRange> result = new List<FixedRange>();
            int visibleRangeEnd = visibleRange.Start + visibleRange.Length;
            foreach (FixedRange range in ranges)
            {
                if (visibleRangeEnd < range.Start)
                    break;

                if (visibleRange.Contains(range))
                    result.Add(range);
            }
            return result;
        }
    }
}
