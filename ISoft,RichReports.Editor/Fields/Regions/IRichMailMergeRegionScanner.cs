using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    public interface IRichMailMergeRegionScanner : IDisposable
    {
        MailMergeRegionInfo this[int index] { get; }

        MailMergeRegionInfo this[string regionName] { get; }

        int Count { get; }

        bool IsEmpty { get; }

        MailMergeRegionInfo Root { get; }

        RegionTreeNode BuildRegionTree();

        bool ContainsRegion(string regionName);

        RegionExportData ExportData();

        IRichReadOnlyRegionCollection FilterBy(Func<MailMergeRegionInfo, bool> predicate);

        IRichReadOnlyRegionCollection FindAllByName(string regionName);

        MailMergeRegionInfo FindByName(string regionName);

        MailMergeRegionInfo FindByPosition(int position);

        IRichReadOnlyRegionCollection FindRegionsContainingPosition(int position);

        IRichReadOnlyRegionCollection FindRegionsInRange(int startPosition, int endPosition, RegionSearchMode mode = RegionSearchMode.FullyContained);

        IRichReadOnlyRegionCollection GetAll();

        IReadOnlyList<string> GetAllRegionNames();

        string GetAllRegionPath();

        IRichReadOnlyRegionCollection GetInvalidRegions();

        int GetMaxNestingDepth();

        IRichReadOnlyRegionCollection GetNestedRegions(MailMergeRegionInfo parentRegion);

        IRichReadOnlyRegionCollection GetParentRegions(MailMergeRegionInfo region);

        string GetRegionPath(MailMergeRegionInfo region);

        FixedRange GetRegionRange(MailMergeRegionInfo regionInfo);

        IRichReadOnlyRegionCollection GetRegionsByDepth(int depth);

        IRichReadOnlyRegionCollection GetRegionsByParent(MailMergeRegionInfo parentRegion);

        string SaveExportData();

        IRichReadOnlyRegionCollection GetRegionsWithMergeFields();

        IRichReadOnlyRegionCollection GetRegionsWithoutMergeFields();

        IRichReadOnlyRegionCollection GetRootRegions();

        IRichReadOnlyRegionCollection GetSiblingRegions(MailMergeRegionInfo region);

        RegionStatistics GetStatistics();

        IRichReadOnlyRegionCollection GetValidRegions();

        Dictionary<string, List<MailMergeRegionInfo>> GroupByName();

        Dictionary<int, List<MailMergeRegionInfo>> GroupByNestingLevel();

        string PrintHierarchy(int indent = 0);

        RegionValidationResult Validate();

        bool IsRegionInRange(MailMergeRegionInfo region, int targetStart, int targetEnd, RegionSearchMode mode = RegionSearchMode.FullyContained);
    }
}
