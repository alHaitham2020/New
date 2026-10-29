using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    public interface IMailMergeDocumentRegionScanner : IDisposable
    {
        bool HasRegions { get; }
        bool IsInitialized { get; }
        DateTime LastScanTime { get; }
        int RegionCount { get; }
        IRichMailMergeRegionScanner Scanner { get; }

        RegionTreeNode BuildRegionTree();
        void CleanupInvalidRegions(Document document);
        bool ContainsRegion(string regionName);
        RegionExportData ExportData();
        Task<RegionExportData> ExportDataAsync(CancellationToken cancellationToken = default);
        string ExportToJson();
        Task<string> ExportToJsonAsync(CancellationToken cancellationToken = default);
        IRichReadOnlyRegionCollection Filter(Func<MailMergeRegionInfo, bool> predicate);
        Task<IRichReadOnlyRegionCollection> FilterAsync(Func<MailMergeRegionInfo, bool> predicate, CancellationToken cancellationToken = default);
        MailMergeRegionInfo FindRegionByPosition(int position);
        MailMergeRegionInfo FindRegionInRow(TableRow row, string regionName);
        IRichReadOnlyRegionCollection FindRegionsContainingPosition(int position);
        IRichReadOnlyRegionCollection FindRegionsInRange(DocumentRange range, RegionSearchMode mode = RegionSearchMode.Overlapping);
        IRichReadOnlyRegionCollection FindRegionsInRange(int startPosition, int endPosition, RegionSearchMode mode = RegionSearchMode.FullyContained);
        List<string> GetAllMergeFieldNames();
        IReadOnlyList<string> GetAllRegionNames();
        IRichReadOnlyRegionCollection GetAllRegions();
        Task<IRichReadOnlyRegionCollection> GetAllRegionsAsync(CancellationToken cancellationToken = default);
        IRichReadOnlyRegionCollection GetInvalidRegions();
        int GetMaxNestingDepth();
        List<string> GetMergeFieldNames(MailMergeRegionInfo region);
        List<Field> GetMergeFieldsInRegion(MailMergeRegionInfo region);
        IRichReadOnlyRegionCollection GetNestedRegions(MailMergeRegionInfo parentRegion);
        IRichReadOnlyRegionCollection GetParentRegions(MailMergeRegionInfo region);
        MailMergeRegionInfo GetRegionByName(string regionName);
        string GetRegionPath(MailMergeRegionInfo region);
        FixedRange GetRegionRange(MailMergeRegionInfo regionInfo);
        IRichReadOnlyRegionCollection GetRegionsByDepth(int depth);
        IRichReadOnlyRegionCollection GetRegionsByMergeField(string mergeFieldName);
        IRichReadOnlyRegionCollection GetRegionsByName(string regionName);
        IRichReadOnlyRegionCollection GetRegionsByParent(MailMergeRegionInfo parentRegion);
        IRichReadOnlyRegionCollection GetRegionsByValidationStatus(bool isValid);
        IRichReadOnlyRegionCollection GetRegionsInRow(TableRow row);
        IRichReadOnlyRegionCollection GetRegionsWithMergeFields();
        IRichReadOnlyRegionCollection GetRegionsWithoutMergeFields();
        IRichReadOnlyRegionCollection GetRootRegions();
        RowRegionInfo GetRowRegionInfo(TableRow row);
        List<TableRow> GetRowsWithRegions(Table table);
        IRichReadOnlyRegionCollection GetSiblingRegions(MailMergeRegionInfo region);
        RegionStatistics GetStatistics();
        Task<RegionStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
        IRichReadOnlyRegionCollection GetValidRegions();
        bool IsRowHasRegion(TableRow row, string regionName);
        bool IsRowHasRegions(TableRow row);
        string PrintHierarchy(int indent = 0);
        Task<string> PrintHierarchyAsync(int indent = 0, CancellationToken cancellationToken = default);
        void ReScan();
        Task ReScanAsync(CancellationToken cancellationToken = default);
        void Scan();
        Task ScanAsync(CancellationToken cancellationToken = default);
        RegionValidationResult ValidateRegions();
        Task<RegionValidationResult> ValidateRegionsAsync(CancellationToken cancellationToken = default);
        TableValidationResult ValidateTableRegions(Table table);
    }
}
