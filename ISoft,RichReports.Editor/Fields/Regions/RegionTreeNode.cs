using DevExpress.XtraRichEdit.API.Native;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Tree node for region hierarchy
    /// </summary>
    public class RegionTreeNode
    {
        public MailMergeRegionInfo Region { get; set; }

        public List<RegionTreeNode> Children { get; set; } = new List<RegionTreeNode>();

        public int Depth { get; set; }

        public bool HasMergeFields => Region?.MergeFields?.Count > 0;

        public int MergeFieldCount => Region?.MergeFields?.Count ?? 0;
    }
}
