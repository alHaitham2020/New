using DevExpress.XtraRichEdit.API.Native;
using ISoft.Utils.Data.Collection;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    public interface IRichReadOnlyRegionCollection : IReadOnlyDataCollection<MailMergeRegionInfo>
    {
        MailMergeRegionInfo this[int index] { get; }

        MailMergeRegionInfo this[string name] {  get; }

        MailMergeRegionInfo FindRegionByName(string name);
    }

    public class RichReadOnlyRegionCollection : List<MailMergeRegionInfo>, IRichReadOnlyRegionCollection
    {
        public RichReadOnlyRegionCollection()
        {
            
        }

        public RichReadOnlyRegionCollection(IEnumerable<MailMergeRegionInfo> collection) : base(collection)
        {

        }

        public RichReadOnlyRegionCollection(int capacity) : base(capacity)
        {
            
        }

        public MailMergeRegionInfo FindRegionByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            var count = Count;
            for (int i = 0; i < count; i++)
            {
                MailMergeRegionInfo region = this[i];
                if (string.Equals(region.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return region;
                }
            }
            return null;
        }

        public MailMergeRegionInfo this[string name] => FindRegionByName(name);
    }
}
