using DevExpress.XtraRichEdit.API.Native;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Discovers and indexes mail merge regions from a root region with async support
    /// </summary>
    public class RichMailMergeRegionScanner : IRichMailMergeRegionScanner
    {
        #region Private Fields

        private readonly MailMergeRegionInfo _root;
        private readonly List<MailMergeRegionInfo> _regions = new();
        private readonly Dictionary<string, List<MailMergeRegionInfo>> _nameIndex = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, MailMergeRegionInfo> _positionIndex = new();
        private readonly Dictionary<MailMergeRegionInfo, int> _depthCache = new();
        private readonly SemaphoreSlim _scanSemaphore = new(1, 1);
        private volatile bool _isIndexed = false;
        private volatile bool _isScanning = false;
        private DateTime _lastScanned = DateTime.MinValue;

        #endregion

        #region Constructors

        public RichMailMergeRegionScanner(MailMergeRegionInfo root, bool autoScan = true)
        {
            _root = root ?? throw new ArgumentNullException(nameof(root));

            if (autoScan)
            {
                Scan();
            }
        }

        #endregion

        #region Properties

        public int Count => _regions.Count;

        public bool IsEmpty => _regions.Count == 0;

        public bool IsIndexed => _isIndexed;

        public DateTime LastScanned => _lastScanned;

        public MailMergeRegionInfo Root => _root;

        public MailMergeRegionInfo this[int index]
        {
            get
            {
                if (index < 0 || index >= _regions.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));
                return _regions[index];
            }
        }

        public MailMergeRegionInfo this[string regionName] => FindByName(regionName);

        #endregion

        #region Public Scan Methods

        /// <summary>
        /// Scans and indexes all regions synchronously
        /// </summary>
        public void Scan()
        {
            if (_isIndexed && !_isScanning) return;

            _scanSemaphore.Wait();
            try
            {
                _isScanning = true;
                try
                {
                    DiscoverRegions();
                    BuildIndexes();
                    _lastScanned = DateTime.Now;
                    _isIndexed = true;
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
        /// Scans and indexes all regions asynchronously
        /// </summary>
        public async Task ScanAsync(CancellationToken cancellationToken = default)
        {
            if (_isIndexed && !_isScanning) return;

            await _scanSemaphore.WaitAsync(cancellationToken);
            try
            {
                _isScanning = true;
                await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        DiscoverRegions();
                        BuildIndexes();
                        _lastScanned = DateTime.Now;
                        _isIndexed = true;
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
        /// Re-scans and re-indexes all regions
        /// </summary>
        public void ReScan()
        {
            _isIndexed = false;
            Scan();
        }

        /// <summary>
        /// Re-scans and re-indexes all regions asynchronously
        /// </summary>
        public async Task ReScanAsync(CancellationToken cancellationToken = default)
        {
            _isIndexed = false;
            await ScanAsync(cancellationToken);
        }

        #endregion

        #region Discovery Methods

        private void DiscoverRegions()
        {
            _regions.Clear();
            CollectAllRegionsRecursive(_root, _regions);
        }

        private void CollectAllRegionsRecursive(MailMergeRegionInfo currentRegion, List<MailMergeRegionInfo> result)
        {
            if (currentRegion == null) return;

            // Add the current region if it has a name or fields
            if (!string.IsNullOrEmpty(currentRegion.Name) || currentRegion.StartField != null)
            {
                result.Add(currentRegion);
            }

            // Process child regions
            if (currentRegion.Regions != null)
            {
                foreach (var childRegion in currentRegion.Regions)
                {
                    CollectAllRegionsRecursive(childRegion, result);
                }
            }
        }

        #endregion

        #region Index Building

        private void BuildIndexes()
        {
            _nameIndex.Clear();
            _positionIndex.Clear();
            _depthCache.Clear();

            foreach (var region in _regions)
            {
                // Add to name index
                if (!string.IsNullOrEmpty(region.Name))
                {
                    if (!_nameIndex.TryGetValue(region.Name, out var list))
                    {
                        list = new List<MailMergeRegionInfo>();
                        _nameIndex[region.Name] = list;
                    }
                    list.Add(region);
                }

                // Add to position index
                if (region.StartField != null)
                {
                    try
                    {
                        int position = region.StartField.Range.Start.ToInt();
                        _positionIndex.Add(position, region);
                    }
                    catch { /* Skip if position can't be determined */ }
                }

                // Cache depth
                _depthCache[region] = CalculateDepth(region);
            }
        }

        private int CalculateDepth(MailMergeRegionInfo region)
        {
            int depth = 0;
            var current = region;
            while (current.ParentRegion != null)
            {
                depth++;
                current = current.ParentRegion;
            }
            return depth;
        }

        private void EnsureIndexed()
        {
            if (!_isIndexed)
            {
                Scan();
            }
        }

        #endregion

        #region Lookup Methods

        public MailMergeRegionInfo FindByName(string regionName)
        {
            if (string.IsNullOrEmpty(regionName)) return null;


            EnsureIndexed();

            if (_nameIndex.TryGetValue(regionName, out var regions))
            {
                return regions.FirstOrDefault();
            }

            return _regions.FirstOrDefault(r => string.Equals(r.Name, regionName, StringComparison.OrdinalIgnoreCase));
        }

        public IRichReadOnlyRegionCollection FindAllByName(string regionName)
        {
            if (string.IsNullOrEmpty(regionName)) return new RichReadOnlyRegionCollection();

            EnsureIndexed();

            if (_nameIndex.TryGetValue(regionName, out var regions))
            {
                return new RichReadOnlyRegionCollection(regions.ToList());
            }

            return FilterBy(r => string.Equals(r.Name, regionName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IRichReadOnlyRegionCollection> FindAllByNameAsync(string regionName, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => FindAllByName(regionName), cancellationToken);
        }

        public MailMergeRegionInfo FindByPosition(int position)
        {
            EnsureIndexed();

            if (_positionIndex.TryGetValue(position, out var region))
            {
                return region;
            }

            return _regions.FirstOrDefault(r =>
            {
                try
                {
                    return r.StartField != null &&
                           r.StartField.Range.Start.ToInt() == position;
                }
                catch { return false; }
            });
        }

        public IRichReadOnlyRegionCollection FindRegionsContainingPosition(int position)
        {
            return FilterBy(r =>
            {
                try
                {
                    if (r.StartField == null || r.EndField == null)
                        return false;

                    int start = r.StartField.Range.Start.ToInt();
                    int end = r.EndField.Range.End.ToInt();
                    return position >= start && position <= end;
                }
                catch { return false; }
            });
        }

        public IRichReadOnlyRegionCollection FindRegionsInRange(
            int startPosition,
            int endPosition,
            RegionSearchMode mode = RegionSearchMode.FullyContained)
        {
            return FilterBy(r =>
            {
                try
                {
                    if (r.StartField == null || r.EndField == null)
                        return false;

                    int regionStart = r.StartField.Range.Start.ToInt();
                    int regionEnd = r.EndField.Range.End.ToInt();

                    return mode switch
                    {
                        RegionSearchMode.Overlapping => regionStart <= endPosition && regionEnd >= startPosition,
                        RegionSearchMode.FullyContained => regionStart >= startPosition && regionEnd <= endPosition,
                        RegionSearchMode.Containing => regionStart <= startPosition && regionEnd >= endPosition,
                        _ => false
                    };
                }
                catch { return false; }
            });
        }

        public bool IsRegionInRange(
            MailMergeRegionInfo region,
            int targetStart,
            int targetEnd,
            RegionSearchMode mode = RegionSearchMode.FullyContained)
        {
            if (region.StartField == null || region.EndField == null)
                return false;

            try
            {
                int startPos = region.StartField.Range.Start.ToInt();
                int endPos = region.EndField.Range.End.ToInt();

                return mode switch
                {
                    RegionSearchMode.Overlapping => startPos <= targetEnd && endPos >= targetStart,
                    RegionSearchMode.FullyContained => startPos >= targetStart && endPos <= targetEnd,
                    RegionSearchMode.Containing => startPos <= targetStart && endPos >= targetEnd,
                    _ => false,
                };
            }
            catch
            {
                return false;
            }
        }

        public bool ContainsRegion(string regionName)
        {
            return FindByName(regionName) != null;
        }

        public IReadOnlyList<string> GetAllRegionNames()
        {
            return _regions
                   .Where(r => !string.IsNullOrEmpty(r.Name))
                   .Select(r => r.Name)
                   .Distinct(StringComparer.OrdinalIgnoreCase)
                   .ToList();
        }

        public Dictionary<string, List<MailMergeRegionInfo>> GroupByName()
        {
            return _regions
                   .Where(r => !string.IsNullOrEmpty(r.Name))
                   .GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                   .ToDictionary(
                       g => g.Key,
                       g => g.ToList(),
                       StringComparer.OrdinalIgnoreCase);
        }

        public IRichReadOnlyRegionCollection GetAll()
        {
            return new RichReadOnlyRegionCollection(_regions.ToList());
        }

        public async Task<IRichReadOnlyRegionCollection> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetAll(), cancellationToken);
        }

        #endregion

        #region Hierarchy Methods

        public RegionTreeNode BuildRegionTree()
        {
            var rootNode = new RegionTreeNode { Region = _root, Depth = 0 };
            BuildTreeRecursive(rootNode, 1);
            return rootNode;
        }

        public IRichReadOnlyRegionCollection GetRegionsByParent(MailMergeRegionInfo parentRegion)
        {
            if (parentRegion == null) return new RichReadOnlyRegionCollection();

            return FilterBy(r => r.ParentRegion == parentRegion);

        }

        public IRichReadOnlyRegionCollection GetRootRegions()
        {
            return FilterBy(r => r.ParentRegion == null);
        }

        public IRichReadOnlyRegionCollection GetRegionsByDepth(int depth)
        {
            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth cannot be negative.");

            EnsureIndexed();
            return FilterBy(r => _depthCache.TryGetValue(r, out int d) && d == depth);
        }

        public Dictionary<int, List<MailMergeRegionInfo>> GroupByNestingLevel()
        {
            EnsureIndexed();
            return _regions
                .GroupBy(r => _depthCache[r])
                .ToDictionary(
                    g => g.Key,
                    g => g.ToList());
        }

        public int GetMaxNestingDepth()
        {
            EnsureIndexed();
            return _depthCache.Count == 0 ? 0 : _depthCache.Values.Max();
        }

        public string GetAllRegionPath()
        {
            var pathSegments = new List<string>();
            var current = _root;

            while (current != null)
            {
                if (!string.IsNullOrEmpty(current.Name))
                {
                    pathSegments.Insert(0, current.Name);
                }
                current = current.ParentRegion;
            }
            return string.Join(" > ", pathSegments);
        }

        public IRichReadOnlyRegionCollection GetNestedRegions(MailMergeRegionInfo parentRegion)
        {
            if (parentRegion == null) throw new ArgumentNullException(nameof(parentRegion));

            var nestedRegions = new RichReadOnlyRegionCollection();
            if (parentRegion.Regions != null)
            {
                foreach (var child in parentRegion.Regions)
                {
                    nestedRegions.Add(child);
                    nestedRegions.AddRange(GetNestedRegions(child));
                }
            }
            return nestedRegions;
        }

        public IRichReadOnlyRegionCollection GetParentRegions(MailMergeRegionInfo region)
        {
            if (region == null) throw new ArgumentNullException(nameof(region));

            var parents = new RichReadOnlyRegionCollection();
            var current = region.ParentRegion;
            while (current != null)
            {
                parents.Add(current);
                current = current.ParentRegion;
            }
            return parents;
        }

        public IRichReadOnlyRegionCollection GetSiblingRegions(MailMergeRegionInfo region)
        {
            if (region == null) throw new ArgumentNullException(nameof(region));

            var siblings = new RichReadOnlyRegionCollection();
            var parent = region.ParentRegion;
            if (parent?.Regions != null)
            {
                siblings.AddRange(parent.Regions.Where(sibling => sibling != region));
            }
            return siblings;
        }

        #endregion

        #region Filter Methods

        public IRichReadOnlyRegionCollection FilterBy(Func<MailMergeRegionInfo, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var filteredRegions = _regions.Where(predicate).ToList();
            return new RichReadOnlyRegionCollection(filteredRegions);
        }

        public async Task<IRichReadOnlyRegionCollection> FilterByAsync(Func<MailMergeRegionInfo, bool> predicate, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => FilterBy(predicate), cancellationToken);
        }

        public RichMailMergeRegionScanner Filter(Func<MailMergeRegionInfo, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var filteredRegions = _regions.Where(predicate).ToList();
            var root = filteredRegions.FirstOrDefault()?.ParentRegion ?? _root;
            var scanner = new RichMailMergeRegionScanner(root, false);
            scanner._regions.Clear();
            scanner._regions.AddRange(filteredRegions);
            scanner.BuildIndexes();
            scanner._isIndexed = true;
            return scanner;
        }

        public IRichReadOnlyRegionCollection GetRegionsWithMergeFields()
        {
            return FilterBy(r => r.MergeFields?.Count > 0);

        }

        public IRichReadOnlyRegionCollection GetRegionsWithoutMergeFields()
        {
            return FilterBy(r => r.MergeFields == null || r.MergeFields.Count == 0);
        }

        public IRichReadOnlyRegionCollection GetValidRegions()
        {
            return FilterBy(r => r.StartField != null && r.EndField != null);
        }

        public IRichReadOnlyRegionCollection GetInvalidRegions()
        {
            return FilterBy(r => r.StartField == null || r.EndField == null);
        }

        #endregion

        #region Validation Methods

        public RegionValidationResult Validate()
        {
            var result = new RegionValidationResult
            {
                TotalRegionsChecked = _regions.Count
            };

            if (_regions.Count == 0)
            {
                result.Warnings.Add(new RegionValidationWarning
                {
                    RegionName = "Root",
                    Message = "No mail merge regions found",
                    Type = RegionValidationWarning.WarningType.NoMergeFields
                });
                return result;
            }

            // Check for duplicate region names (optimized)
            var duplicates = _regions
                .Where(r => !string.IsNullOrEmpty(r.Name))
                .GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                result.Warnings.Add(new RegionValidationWarning
                {
                    RegionName = duplicate.Key,
                    Message = $"Duplicate region name found: '{duplicate.Key}' ({duplicate.Count()} occurrences)",
                    Type = RegionValidationWarning.WarningType.DuplicateName
                });
            }

            // Validate each region
            foreach (var region in _regions)
            {
                ValidateRegion(region, result);
            }

            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        public async Task<RegionValidationResult> ValidateAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => Validate(), cancellationToken);
        }

        private void ValidateRegion(MailMergeRegionInfo region, RegionValidationResult result)
        {
            string regionName = region.Name ?? "Unnamed";

            if (region.StartField == null)
            {
                result.Errors.Add(new RegionValidationError
                {
                    RegionName = regionName,
                    Message = "Region is missing its start field",
                    Type = RegionValidationError.ErrorType.MissingStartField
                });
            }

            if (region.EndField == null)
            {
                result.Errors.Add(new RegionValidationError
                {
                    RegionName = regionName,
                    Message = "Region is missing its end field",
                    Type = RegionValidationError.ErrorType.MissingEndField
                });
            }

            if (string.IsNullOrEmpty(region.Name))
            {
                result.Errors.Add(new RegionValidationError
                {
                    RegionName = regionName,
                    Message = "Region has no name specified",
                    Type = RegionValidationError.ErrorType.InvalidName
                });
            }

            bool hasMergeFields = region.MergeFields?.Count > 0;
            bool hasChildren = region.Regions?.Count > 0;

            if (!hasMergeFields && !hasChildren)
            {
                result.Warnings.Add(new RegionValidationWarning
                {
                    RegionName = regionName,
                    Message = "Region is empty (no merge fields or child regions)",
                    Type = RegionValidationWarning.WarningType.EmptyNestedRegions
                });
            }
        }

        #endregion

        #region Statistics Methods

        public RegionStatistics GetStatistics()
        {
            EnsureIndexed();

            return new RegionStatistics
            {
                TotalRegions = _regions.Count,
                TotalMergeFields = _regions.Sum(r => r.MergeFields?.Count ?? 0),
                TotalNestedRegions = _regions.Count(r => r.ParentRegion != null),
                MaxNestingDepth = GetMaxNestingDepth(),
                RegionNames = GetAllRegionNames().ToList(),
                MergeFieldsPerRegion = _regions
                    .Where(r => !string.IsNullOrEmpty(r.Name))
                    .ToDictionary(
                        r => r.Name ?? "Unnamed",
                        r => r.MergeFields?.Count ?? 0)
            };
        }

        public async Task<RegionStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetStatistics(), cancellationToken);
        }

        #endregion

        #region Export Methods

        public RegionExportData ExportData()
        {
            return new RegionExportData
            {
                Statistics = GetStatistics(),
                Regions = _regions.Select(CreateExportItem).ToList()
            };
        }

        public async Task<RegionExportData> ExportDataAsync(CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => ExportData(), cancellationToken);
        }

        public string SaveExportData()
        {
            return JsonSerializationHelper.SerializeToJson(ExportData());
        }

        public async Task<string> SaveExportDataAsync(CancellationToken cancellationToken = default)
        {
            var data = await ExportDataAsync(cancellationToken);
            return JsonSerializationHelper.SerializeToJson(data);
        }

        private RegionExportItem CreateExportItem(MailMergeRegionInfo region)
        {
            var item = new RegionExportItem
            {
                Name = region.Name ?? "Unnamed",
                ParentName = region.ParentRegion?.Name ?? "Root",
                MergeFieldCount = region.MergeFields?.Count ?? 0,
                NestedRegionCount = region.Regions?.Count ?? 0,
                Depth = _depthCache.TryGetValue(region, out int depth) ? depth : 0,
                IsValid = region.StartField != null && region.EndField != null
            };

            if (region.StartField != null)
            {
                try { item.StartPosition = region.StartField.Range.Start.ToInt(); }
                catch { /* Ignore */ }
            }

            if (region.EndField != null)
            {
                try { item.EndPosition = region.EndField.Range.End.ToInt(); }
                catch { /* Ignore */ }
            }

            if (!item.IsValid)
            {
                if (region.StartField == null) item.ValidationMessages.Add("Missing start field");
                if (region.EndField == null) item.ValidationMessages.Add("Missing end field");
            }

            return item;
        }

        #endregion

        #region Utility Methods

        public RichMailMergeRegionScanner Clone()
        {
            var scanner = new RichMailMergeRegionScanner(_root, false);
            scanner._regions.Clear();
            scanner._regions.AddRange(_regions);
            scanner.BuildIndexes();
            scanner._isIndexed = true;
            scanner._lastScanned = _lastScanned;
            return scanner;
        }

        public FixedRange GetRegionRange(MailMergeRegionInfo regionInfo)
        {
            if (regionInfo?.StartField == null || regionInfo.EndField == null)
                throw new ArgumentException("Region has invalid start or end fields");

            int start = regionInfo.StartField.Range.GetAsFrozen().Start.ToInt();
            int end = regionInfo.EndField.Range.GetAsFrozen().End.ToInt();
            return new FixedRange(start, end - start);
        }

        public string GetRegionPath(MailMergeRegionInfo region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            var pathSegments = new List<string>();
            var current = region;
            while (current != null)
            {
                if (!string.IsNullOrEmpty(current.Name))
                {
                    pathSegments.Insert(0, current.Name);
                }
                current = current.ParentRegion;
            }
            return string.Join(" > ", pathSegments);
        }

        public string PrintHierarchy(int indent = 0)
        {
            var result = new System.Text.StringBuilder();
            PrintHierarchyRecursive(_root, indent, result);
            return result.ToString();
        }

        public async Task<string> PrintHierarchyAsync(int indent = 0, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => PrintHierarchy(indent), cancellationToken);
        }

        private void PrintHierarchyRecursive(MailMergeRegionInfo region, int indent, System.Text.StringBuilder result)
        {
            string indentStr = new string(' ', indent * 2);
            result.AppendLine($"{indentStr}  Region: {region?.Name ?? "Root"}");
            result.AppendLine($"{indentStr}  Fields: {region?.MergeFields?.Count ?? 0}");

            if (region?.Regions != null)
            {
                foreach (var child in region.Regions)
                {
                    PrintHierarchyRecursive(child, indent + 1, result);
                }
            }
        }

        private void BuildTreeRecursive(RegionTreeNode node, int depth)
        {
            if (node.Region == null)
                return;

            var childRegions = node.Region.Regions;
            if (childRegions != null)
            {
                foreach (var childRegion in childRegions)
                {
                    var childNode = new RegionTreeNode
                    {
                        Region = childRegion,
                        Depth = depth
                    };
                    node.Children.Add(childNode);
                    BuildTreeRecursive(childNode, depth + 1);
                }
            }
        }

        #endregion

        #region IReadOnlyRegionCollection Implementation

        public void Add(MailMergeRegionInfo item)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        public void AddRange(IEnumerable<MailMergeRegionInfo> items)
        {
            throw new NotSupportedException("This collection is read-only");
        }

        public void Clear()
        {
            throw new NotSupportedException("This collection is read-only");
        }

        public bool Contains(MailMergeRegionInfo item)
        {
            return _regions.Contains(item);
        }

        public void CopyTo(MailMergeRegionInfo[] array, int arrayIndex)
        {
            _regions.CopyTo(array, arrayIndex);
        }

        //public IEnumerator<MailMergeRegionInfo> GetEnumerator()
        //{
        //    _lock.EnterReadLock();
        //    try
        //    {
        //        return _regions.ToList().GetEnumerator();
        //    }
        //    finally
        //    {
        //        _lock.ExitReadLock();
        //    }
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return GetEnumerator();
        //}

        #endregion

        #region IDisposable

        public void Dispose()
        {
            _scanSemaphore?.Dispose();
        }

        #endregion
    }
}
