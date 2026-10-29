using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Document-level scanner for mail merge regions with caching and async support
    /// </summary>
    public class MailMergeDocumentRegionScanner : IMailMergeDocumentRegionScanner
    {
        #region Private Fields

        private readonly Document _document;
        private readonly string _regionStartTag;
        private readonly string _regionEndTag;
        private readonly SemaphoreSlim _scanSemaphore = new(1, 1);

        private RichMailMergeRegionScanner _scanner;
        private bool _isDisposed;
        private DateTime _lastScanTime = DateTime.MinValue;
        private bool _isScanning;

        public bool IsScanning => _isScanning;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the MailMergeDocumentRegionScanner class
        /// </summary>
        /// <param name="document">The document to scan</param>
        /// <param name="regionStartTag">Optional custom region start tag</param>
        /// <param name="regionEndTag">Optional custom region end tag</param>
        /// <param name="autoScan">Whether to automatically scan on construction</param>
        public MailMergeDocumentRegionScanner(
            Document document,
            bool autoScan = true,
            string regionStartTag = null,
            string regionEndTag = null)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
            _regionStartTag = regionStartTag;
            _regionEndTag = regionEndTag;

            if (autoScan)
            {
                Scan();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets whether the scanner has been initialized
        /// </summary>
        public bool IsInitialized => _scanner != null && _scanner.IsIndexed;

        /// <summary>
        /// Gets the last scan time
        /// </summary>
        public DateTime LastScanTime => _lastScanTime;

        /// <summary>
        /// Gets the total number of regions found
        /// </summary>
        public int RegionCount => _scanner?.Count ?? 0;

        /// <summary>
        /// Gets whether the document has any regions
        /// </summary>
        public bool HasRegions => RegionCount > 0;

        /// <summary>
        /// Gets the underlying scanner instance
        /// </summary>
        public IRichMailMergeRegionScanner Scanner => _scanner;

        #endregion

        #region Public Scan Methods

        /// <summary>
        /// Scans the document synchronously
        /// </summary>
        public void Scan()
        {
            ThrowIfDisposed();

            _scanSemaphore.Wait();
            try
            {
                _isScanning = true;
                try
                {
                    _scanner = CreateScanner();
                    _scanner.Scan();
                    _lastScanTime = DateTime.Now;
                }
                finally
                {
                    _isScanning = false;
                }
            }
            finally
            {
                _scanSemaphore.Release();
            }
        }

        /// <summary>
        /// Scans the document asynchronously
        /// </summary>
        public async Task ScanAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            await _scanSemaphore.WaitAsync(cancellationToken);
            try
            {
                _isScanning = true;
                await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        _scanner = CreateScanner();
                        _scanner.Scan();
                        _lastScanTime = DateTime.Now;
                    }
                    finally
                    {
                        _isScanning = false;
                    }
                }, cancellationToken);
            }
            finally
            {
                _scanSemaphore.Release();
            }
        }

        /// <summary>
        /// Re-scans the document
        /// </summary>
        public void ReScan()
        {
            ThrowIfDisposed();
            _scanner = null;
            Scan();
        }

        /// <summary>
        /// Re-scans the document asynchronously
        /// </summary>
        public async Task ReScanAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            _scanner = null;
            await ScanAsync(cancellationToken);
        }

        #endregion

        #region Private Helper Methods

        private RichMailMergeRegionScanner CreateScanner()
        {
            var rootRegion = string.IsNullOrEmpty(_regionStartTag) || string.IsNullOrEmpty(_regionEndTag)
                ? _document.GetRegionHierarchy()
                : _document.GetRegionHierarchy(_regionStartTag, _regionEndTag);

            return new RichMailMergeRegionScanner(rootRegion, true);
        }

        private void EnsureScannerInitialized()
        {
            if (_scanner == null || !_scanner.IsIndexed)
            {
                Scan();
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(MailMergeDocumentRegionScanner));
        }

        private T ExecuteSafe<T>(Func<T> action)
        {
            ThrowIfDisposed();
            EnsureScannerInitialized();

            return action();
        }

        private void ExecuteSafe(Action action)
        {
            ThrowIfDisposed();
            EnsureScannerInitialized();

            action();
        }

        #endregion

        #region Public Lookup Methods

        /// <summary>
        /// Gets all regions in the document
        /// </summary>
        public IRichReadOnlyRegionCollection GetAllRegions()
        {
            return ExecuteSafe(() => _scanner.GetAll());
        }

        /// <summary>
        /// Gets all regions asynchronously
        /// </summary>
        public async Task<IRichReadOnlyRegionCollection> GetAllRegionsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetAllRegions(), cancellationToken);
        }

        /// <summary>
        /// Finds a region by name
        /// </summary>
        public MailMergeRegionInfo GetRegionByName(string regionName)
        {
            return ExecuteSafe(() => _scanner.FindByName(regionName));
        }

        /// <summary>
        /// Finds all regions by name
        /// </summary>
        public IRichReadOnlyRegionCollection GetRegionsByName(string regionName)
        {
            return ExecuteSafe(() => _scanner.FindAllByName(regionName));
        }

        /// <summary>
        /// Finds regions within a specific document range
        /// </summary>
        public IRichReadOnlyRegionCollection FindRegionsInRange(
            DocumentRange range,
            RegionSearchMode mode = RegionSearchMode.FullyContained)
        {
            if (range == null)
                throw new ArgumentNullException(nameof(range));

            return ExecuteSafe(() =>
            {
                int start = range.Start.ToInt();
                int end = range.End.ToInt();
                return _scanner.FindRegionsInRange(start, end, mode);
            });
        }

        /// <summary>
        /// Finds regions within a specific position range
        /// </summary>
        public IRichReadOnlyRegionCollection FindRegionsInRange(
            int startPosition,
            int endPosition,
            RegionSearchMode mode = RegionSearchMode.FullyContained)
        {
            return ExecuteSafe(() =>
                _scanner.FindRegionsInRange(startPosition, endPosition, mode));
        }

        /// <summary>
        /// Finds a region by position
        /// </summary>
        public MailMergeRegionInfo FindRegionByPosition(int position)
        {
            return ExecuteSafe(() => _scanner.FindByPosition(position));
        }

        /// <summary>
        /// Finds regions containing a specific position
        /// </summary>
        public IRichReadOnlyRegionCollection FindRegionsContainingPosition(int position)
        {
            return ExecuteSafe(() => _scanner.FindRegionsContainingPosition(position));
        }

        /// <summary>
        /// Checks if a region with the specified name exists
        /// </summary>
        public bool ContainsRegion(string regionName)
        {
            return ExecuteSafe(() => _scanner.ContainsRegion(regionName));
        }

        /// <summary>
        /// Gets all region names
        /// </summary>
        public IReadOnlyList<string> GetAllRegionNames()
        {
            return ExecuteSafe(() => _scanner.GetAllRegionNames());
        }

        #endregion

        #region Hierarchy Methods

        /// <summary>
        /// Builds a tree structure of regions
        /// </summary>
        public RegionTreeNode BuildRegionTree()
        {
            return ExecuteSafe(() => _scanner.BuildRegionTree());
        }

        /// <summary>
        /// Gets root regions (regions with no parent)
        /// </summary>
        public IRichReadOnlyRegionCollection GetRootRegions()
        {
            return ExecuteSafe(() => _scanner.GetRootRegions());
        }

        /// <summary>
        /// Gets regions by parent
        /// </summary>
        public IRichReadOnlyRegionCollection GetRegionsByParent(MailMergeRegionInfo parentRegion)
        {
            return ExecuteSafe(() => _scanner.GetRegionsByParent(parentRegion));
        }

        /// <summary>
        /// Gets regions by nesting depth
        /// </summary>
        public IRichReadOnlyRegionCollection GetRegionsByDepth(int depth)
        {
            return ExecuteSafe(() => _scanner.GetRegionsByDepth(depth));
        }

        /// <summary>
        /// Gets nested regions of a parent
        /// </summary>
        public IRichReadOnlyRegionCollection GetNestedRegions(MailMergeRegionInfo parentRegion)
        {
            return ExecuteSafe(() => _scanner.GetNestedRegions(parentRegion));
        }

        /// <summary>
        /// Gets parent regions of a region
        /// </summary>
        public IRichReadOnlyRegionCollection GetParentRegions(MailMergeRegionInfo region)
        {
            return ExecuteSafe(() => _scanner.GetParentRegions(region));
        }

        /// <summary>
        /// Gets sibling regions of a region
        /// </summary>
        public IRichReadOnlyRegionCollection GetSiblingRegions(MailMergeRegionInfo region)
        {
            return ExecuteSafe(() => _scanner.GetSiblingRegions(region));
        }

        /// <summary>
        /// Gets the maximum nesting depth
        /// </summary>
        public int GetMaxNestingDepth()
        {
            return ExecuteSafe(() => _scanner.GetMaxNestingDepth());
        }

        /// <summary>
        /// Gets the path of a region
        /// </summary>
        public string GetRegionPath(MailMergeRegionInfo region)
        {
            return ExecuteSafe(() => _scanner.GetRegionPath(region));
        }

        #endregion

        #region Filter Methods

        /// <summary>
        /// Filters regions by a predicate
        /// </summary>
        public IRichReadOnlyRegionCollection Filter(Func<MailMergeRegionInfo, bool> predicate)
        {
            return ExecuteSafe(() => _scanner.FilterBy(predicate));
        }

        /// <summary>
        /// Filters regions asynchronously
        /// </summary>
        public async Task<IRichReadOnlyRegionCollection> FilterAsync(
            Func<MailMergeRegionInfo, bool> predicate,
            CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => Filter(predicate), cancellationToken);
        }

        /// <summary>
        /// Gets regions with merge fields
        /// </summary>
        public IRichReadOnlyRegionCollection GetRegionsWithMergeFields()
        {
            return ExecuteSafe(() => _scanner.GetRegionsWithMergeFields());
        }

        /// <summary>
        /// Gets regions without merge fields
        /// </summary>
        public IRichReadOnlyRegionCollection GetRegionsWithoutMergeFields()
        {
            return ExecuteSafe(() => _scanner.GetRegionsWithoutMergeFields());
        }

        /// <summary>
        /// Gets valid regions (have both start and end fields)
        /// </summary>
        public IRichReadOnlyRegionCollection GetValidRegions()
        {
            return ExecuteSafe(() => _scanner.GetValidRegions());
        }

        /// <summary>
        /// Gets invalid regions (missing start or end fields)
        /// </summary>
        public IRichReadOnlyRegionCollection GetInvalidRegions()
        {
            return ExecuteSafe(() => _scanner.GetInvalidRegions());
        }

        /// <summary>
        /// Gets regions by validation status
        /// </summary>
        public IRichReadOnlyRegionCollection GetRegionsByValidationStatus(bool isValid)
        {
            return ExecuteSafe(() =>
                isValid ? _scanner.GetValidRegions() : _scanner.GetInvalidRegions());
        }

        /// <summary>
        /// Gets regions by merge field name
        /// </summary>
        public IRichReadOnlyRegionCollection GetRegionsByMergeField(string mergeFieldName)
        {
            if (string.IsNullOrEmpty(mergeFieldName))
                throw new ArgumentException("Merge field name cannot be null or empty", nameof(mergeFieldName));

            return ExecuteSafe(() =>
            {
                var matchingRegions = new List<MailMergeRegionInfo>();
                var allRegions = _scanner.GetAll();

                foreach (var region in allRegions)
                {
                    var fieldNames = GetMergeFieldNames(region);
                    if (fieldNames.Contains(mergeFieldName, StringComparer.OrdinalIgnoreCase))
                    {
                        matchingRegions.Add(region);
                    }
                }

                return new RichReadOnlyRegionCollection(matchingRegions);
            });
        }

        #endregion

        #region Merge Field Methods

        /// <summary>
        /// Gets all merge fields in a region
        /// </summary>
        public List<Field> GetMergeFieldsInRegion(MailMergeRegionInfo region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            return region.MergeFields?.ToList() ?? new List<Field>();
        }

        /// <summary>
        /// Gets merge field names in a region
        /// </summary>
        public List<string> GetMergeFieldNames(MailMergeRegionInfo region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            var fieldNames = new List<string>();

            foreach (var field in region.MergeFields ?? Enumerable.Empty<Field>())
            {
                try
                {
                    var fieldCode = _document.GetText(field.CodeRange);
                    var fieldName = ExtractMergeFieldName(fieldCode);
                    if (!string.IsNullOrEmpty(fieldName))
                    {
                        fieldNames.Add(fieldName);
                    }
                }
                catch
                {
                    // Skip fields that can't be parsed
                }
            }

            return fieldNames.Distinct().ToList();
        }

        /// <summary>
        /// Gets all merge field names in the document
        /// </summary>
        public List<string> GetAllMergeFieldNames()
        {
            return ExecuteSafe(() =>
            {
                var allFieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var allRegions = _scanner.GetAll();

                foreach (var region in allRegions)
                {
                    var fieldNames = GetMergeFieldNames(region);
                    foreach (var name in fieldNames)
                    {
                        allFieldNames.Add(name);
                    }
                }

                return allFieldNames.ToList();
            });
        }

        #endregion

        #region Row Region Methods

        /// <summary>
        /// Checks if a table row contains a region
        /// </summary>
        public bool IsRowHasRegions(TableRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            return ExecuteSafe(() =>
            {
                var rowRange = row.Range;
                int rowStart = rowRange.Start.ToInt();
                int rowEnd = rowRange.End.ToInt();

                var regionsInRow = _scanner.FindRegionsInRange(rowStart, rowEnd);
                return regionsInRow.Any();
            });
        }

        /// <summary>
        /// Checks if a table row contains a region
        /// </summary>
        public bool IsRowHasRegion(TableRow row, string regionName)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if (string.IsNullOrEmpty(regionName)) return false;

            var regionInfo = FindRegionInRow(row,regionName);
            return regionInfo != null;
        }

        /// <summary>
        /// Checks if a table row contains a region
        /// </summary>
        public MailMergeRegionInfo FindRegionInRow(TableRow row,string regionName)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            if(string.IsNullOrEmpty(regionName)) return null;

           return GetRegionsInRow(row).FindRegionByName(regionName);
        }

        public IRichReadOnlyRegionCollection GetRegionsInRow(TableRow row)
        {
            return FindRegionsInRange(row.Range);
        }

        /// <summary>
        /// Gets detailed region info for a table row
        /// </summary>
        public RowRegionInfo GetRowRegionInfo(TableRow row)
        {
            if (row == null)
                throw new ArgumentNullException(nameof(row));

            var result = new RowRegionInfo();

            ExecuteSafe(() =>
            {
                var rowRange = row.Range;
                int rowStart = rowRange.Start.ToInt();
                int rowEnd = rowRange.End.ToInt();

                var regionsInRow = _scanner.FindRegionsInRange(rowStart, rowEnd);

                foreach (var region in regionsInRow)
                {
                    var detail = CreateRegionInfo(region);
                    result.RegionsInRow.Add(detail);
                    result.HasRegion = true;
                }

                // Validate the regions found
                if (result.HasRegion)
                {
                    bool allValid = result.RegionsInRow.All(r => r.HasStartField && r.HasEndField);
                    result.IsValid = allValid;

                    if (!allValid)
                    {
                        var invalidRegions = result.RegionsInRow
                            .Where(r => !r.HasStartField || !r.HasEndField)
                            .Select(r => r.Name);

                        result.ValidationMessage = $"Invalid regions found: {string.Join(", ", invalidRegions)}";
                    }
                    else
                    {
                        result.ValidationMessage = "All regions in row are valid";
                    }
                }
                else
                {
                    result.IsValid = true;
                    result.ValidationMessage = "No regions found in this row";
                }
            });

            return result;
        }

        /// <summary>
        /// Gets all rows with regions in a table
        /// </summary>
        public List<TableRow> GetRowsWithRegions(Table table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            var rowsWithRegions = new List<TableRow>();

            foreach (TableRow row in table.Rows)
            {
                if (IsRowHasRegions(row))
                {
                    rowsWithRegions.Add(row);
                }
            }

            return rowsWithRegions;
        }

        /// <summary>
        /// Validates all regions in a table
        /// </summary>
        public TableValidationResult ValidateTableRegions(Table table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            var result = new TableValidationResult
            {
                TableIndex = GetTableIndex(table),
                TotalRows = table.Rows.Count
            };

            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var rowInfo = GetRowRegionInfo(row);

                result.RowResults.Add(new RowValidationResult
                {
                    RowIndex = i,
                    HasRegion = rowInfo.HasRegion,
                    IsValid = rowInfo.IsValid,
                    Message = rowInfo.ValidationMessage,
                    RegionCount = rowInfo.RegionsInRow.Count
                });

                if (rowInfo.HasRegion && !rowInfo.IsValid)
                {
                    result.HasInvalidRows = true;
                }
            }

            result.IsValid = !result.HasInvalidRows;
            return result;
        }

        #endregion

        #region Validation and Statistics

        /// <summary>
        /// Validates all regions in the document
        /// </summary>
        public RegionValidationResult ValidateRegions()
        {
            return ExecuteSafe(() => _scanner.Validate());
        }

        /// <summary>
        /// Validates all regions asynchronously
        /// </summary>
        public async Task<RegionValidationResult> ValidateRegionsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => ValidateRegions(), cancellationToken);
        }

        /// <summary>
        /// Gets statistics about the regions
        /// </summary>
        public RegionStatistics GetStatistics()
        {
            return ExecuteSafe(() => _scanner.GetStatistics());
        }

        /// <summary>
        /// Gets statistics asynchronously
        /// </summary>
        public async Task<RegionStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetStatistics(), cancellationToken);
        }

        #endregion

        #region Export Methods

        /// <summary>
        /// Exports region data
        /// </summary>
        public RegionExportData ExportData()
        {
            return ExecuteSafe(() => _scanner.ExportData());
        }

        /// <summary>
        /// Exports region data asynchronously
        /// </summary>
        public async Task<RegionExportData> ExportDataAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => ExportData(), cancellationToken);
        }

        /// <summary>
        /// Exports region data as JSON
        /// </summary>
        public string ExportToJson()
        {
            return ExecuteSafe(() => _scanner.SaveExportData());
        }

        /// <summary>
        /// Exports region data as JSON asynchronously
        /// </summary>
        public async Task<string> ExportToJsonAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => ExportToJson(), cancellationToken);
        }

        #endregion

        #region Utility Methods
        /// <summary>
        /// Cleans up invalid or orphaned region fields
        /// </summary>
        public void CleanupInvalidRegions(Document document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            var invalidRegions = GetInvalidRegions();

            if (invalidRegions.Any())
            {
                document.BeginUpdate();
                try
                {
                    foreach (var region in invalidRegions)
                    {
                        try
                        {
                            if (region.StartField != null)
                            {
                                document.Delete(region.StartField.Range);
                            }
                            if (region.EndField != null)
                            {
                                document.Delete(region.EndField.Range);
                            }
                        }
                        catch { /* Ignore individual cleanup errors */ }
                    }
                }
                finally
                {
                    document.EndUpdate();
                }
            }
        }

        /// <summary>
        /// Gets the range of a region
        /// </summary>
        public FixedRange GetRegionRange(MailMergeRegionInfo regionInfo)
        {
            if (regionInfo == null)
                throw new ArgumentNullException(nameof(regionInfo));

            return ExecuteSafe(() => _scanner.GetRegionRange(regionInfo));
        }

        /// <summary>
        /// Prints the region hierarchy for debugging
        /// </summary>
        public string PrintHierarchy(int indent = 0)
        {
            return ExecuteSafe(() => _scanner.PrintHierarchy(indent));
        }

        /// <summary>
        /// Prints the region hierarchy asynchronously
        /// </summary>
        public async Task<string> PrintHierarchyAsync(int indent = 0, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => PrintHierarchy(indent), cancellationToken);
        }

        #endregion

        #region Private Helper Methods

        private RegionInfo CreateRegionInfo(MailMergeRegionInfo region)
        {
            var detail = new RegionInfo
            {
                Name = region.Name ?? "Unnamed",
                HasStartField = region.StartField != null,
                HasEndField = region.EndField != null,
                MergeFieldCount = region.MergeFields?.Count ?? 0
            };

            if (region.StartField != null)
            {
                try 
                {
                    detail.StartPosition = region.StartField.Range.Start.ToInt();
                    detail.StartLength = region.StartField.Range.Length;
                }
                catch { /* Ignore */ }
            }

            if (region.EndField != null)
            {
                try 
                {
                    detail.EndPosition = region.EndField.Range.End.ToInt();
                    detail.EndLength = region.EndField.Range.Length;
                }
                catch { /* Ignore */ }
            }

            // Get merge field names
            if (region.MergeFields != null)
            {
                foreach (var field in region.MergeFields)
                {
                    try
                    {
                        var fieldCode = _document.GetText(field.CodeRange);
                        var fieldName = ExtractMergeFieldName(fieldCode);
                        if (!string.IsNullOrEmpty(fieldName))
                        {
                            detail.MergeFieldNames.Add(fieldName);
                        }
                    }
                    catch { /* Skip fields that can't be parsed */ }
                }
            }

            return detail;
        }

        private string ExtractMergeFieldName(string fieldCode)
        {
            if (string.IsNullOrEmpty(fieldCode))
                return string.Empty;

            fieldCode = fieldCode.Trim().TrimStart('{').TrimEnd('}').Trim();

            const string mergeFieldPrefix = "MERGEFIELD";
            int index = fieldCode.IndexOf(mergeFieldPrefix, StringComparison.OrdinalIgnoreCase);

            if (index < 0)
                return string.Empty;

            string afterPrefix = fieldCode.Substring(index + mergeFieldPrefix.Length).Trim();

            if (afterPrefix.StartsWith("\""))
            {
                int endQuote = afterPrefix.IndexOf("\"", 1);
                if (endQuote > 0)
                {
                    return afterPrefix.Substring(1, endQuote - 1);
                }
            }
            else
            {
                int spaceIndex = afterPrefix.IndexOfAny(new char[] { ' ', '\t', '\r', '\n', '\\' });
                if (spaceIndex > 0)
                {
                    return afterPrefix.Substring(0, spaceIndex);
                }
                else if (!string.IsNullOrEmpty(afterPrefix))
                {
                    return afterPrefix;
                }
            }

            return string.Empty;
        }

        private int GetTableIndex(Table table)
        {
            if (_document?.Tables == null || table == null)
                return -1;

            try
            {
                for (int i = 0; i < _document.Tables.Count; i++)
                {
                    if (_document.Tables[i] == table)
                        return i;
                }
            }
            catch { /* Ignore */ }

            return -1;
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _scanSemaphore?.Dispose();

            if(_scanner!=null)
            {
                _scanner.Dispose();
                _scanner = null;
            }

            _isDisposed = true;
        }

        #endregion
    }
}
