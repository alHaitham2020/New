using ISoft.Utils.Data.Collection;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    public class RegionInfoCollection : DataListBase<RegionInfo>
    {
        public RegionInfoCollection()
        {

        }

        public RegionInfoCollection(IEnumerable<RegionInfo> collection) : base(collection)
        {

        }

        protected override RegionInfo CloneItem(RegionInfo item)
        {
            return (RegionInfo)item.Clone();
        }

        public RegionInfo FindRegionByName(string name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return null;
            }

            var count = Items.Count;
            for (int i = 0; i < count; i++)
            {
                RegionInfo region = Items[i];
                if (string.Equals(region.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return region;
                }
            }
            return null;
        }

        public RegionInfo this[string name] => FindRegionByName(name);
    }
}
