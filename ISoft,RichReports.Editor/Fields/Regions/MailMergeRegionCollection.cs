using DevExpress.Mvvm.Native;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Editor.Fields.Regions
{
    /// <summary>
    /// Specialized collection for MailMergeRegionInfo with enhanced lookup capabilities
    /// </summary>
    public class MailMergeRegionCollection : IList<MailMergeRegionInfo>, IReadOnlyList<MailMergeRegionInfo>
    {
        #region Private Fields

        private readonly List<MailMergeRegionInfo> _regions = new List<MailMergeRegionInfo>();
        private readonly Dictionary<string, List<MailMergeRegionInfo>> _nameIndex = new Dictionary<string, List<MailMergeRegionInfo>>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<int, MailMergeRegionInfo> _positionIndex = new Dictionary<int, MailMergeRegionInfo>();
        private readonly object _lockObject = new object();
        private bool _isIndexDirty = true;

        #endregion

        #region Constructors

        public MailMergeRegionCollection()
        {

        }

        public MailMergeRegionCollection(IEnumerable<MailMergeRegionInfo> regions)
        {
            if (regions != null)
            {
                AddRange(regions);
            }
        }

        #endregion

        #region Properties

        public int Count => _regions.Count;

        public bool IsReadOnly => false;

        public MailMergeRegionInfo this[int index]
        {
            get => _regions[index];
            set
            {
                if (index < 0 || index >= _regions.Count)
                    throw new ArgumentOutOfRangeException(nameof(index));

                lock (_lockObject)
                {
                    // Remove old item from indexes
                    var oldItem = _regions[index];
                    if (oldItem != null)
                    {
                        RemoveFromIndexes(oldItem);
                    }

                    _regions[index] = value;

                    // Add new item to indexes
                    if (value != null)
                    {
                        AddToIndexes(value);
                    }
                }
            }
        }

        public MailMergeRegionInfo this[string regionName]
        {
            get
            {
                return FindByName(regionName);
            }
        }

        #endregion

        #region Index Management

        private void RebuildIndexes()
        {
            lock (_lockObject)
            {
                _nameIndex.Clear();
                _positionIndex.Clear();

                foreach (var region in _regions)
                {
                    AddToIndexes(region);
                }

                _isIndexDirty = false;
            }
        }

        private void AddToIndexes(MailMergeRegionInfo region)
        {
            if (region == null) return;

            // Add to name index
            if (!string.IsNullOrEmpty(region.Name))
            {
                if (!_nameIndex.ContainsKey(region.Name))
                {
                    _nameIndex[region.Name] = new List<MailMergeRegionInfo>();
                }
                _nameIndex[region.Name].Add(region);
            }

            // Add to position index
            try
            {
                if (region.StartField != null)
                {
                    int position = region.StartField.Range.Start.ToInt();
                    if (!_positionIndex.ContainsKey(position))
                    {
                        _positionIndex[position] = region;
                    }
                }
            }
            catch { /* Skip if position can't be determined */ }
        }

        private void RemoveFromIndexes(MailMergeRegionInfo region)
        {
            if (region == null) return;

            // Remove from name index
            if (!string.IsNullOrEmpty(region.Name) && _nameIndex.ContainsKey(region.Name))
            {
                _nameIndex[region.Name].Remove(region);
                if (_nameIndex[region.Name].Count == 0)
                {
                    _nameIndex.Remove(region.Name);
                }
            }

            // Remove from position index
            try
            {
                if (region.StartField != null)
                {
                    int position = region.StartField.Range.Start.ToInt();
                    if (_positionIndex.ContainsKey(position) && _positionIndex[position] == region)
                    {
                        _positionIndex.Remove(position);
                    }
                }
            }
            catch { /* Skip if position can't be determined */ }
        }

        #endregion

        #region Lookup Methods

        /// <summary>
        /// Finds a region by name with case-insensitive comparison
        /// </summary>
        public MailMergeRegionInfo FindByName(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
                return null;

            lock (_lockObject)
            {
                if (_isIndexDirty) RebuildIndexes();

                if (_nameIndex.TryGetValue(regionName, out var regions))
                {
                    return regions.FirstOrDefault();
                }

                // Fallback to linear search if not found in index
                return _regions.FirstOrDefault(r => string.Equals(r.Name, regionName, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Finds all regions by name with case-insensitive comparison
        /// </summary>
        public IReadOnlyList<MailMergeRegionInfo> FindAllByName(string regionName)
        {
            if (string.IsNullOrEmpty(regionName))
                return new List<MailMergeRegionInfo>();

            lock (_lockObject)
            {
                if (_isIndexDirty) RebuildIndexes();

                if (_nameIndex.TryGetValue(regionName, out var regions))
                {
                    return regions.ToList();
                }

                // Fallback to linear search if not found in index
                return _regions.Where(r => string.Equals(r.Name, regionName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
        }

        /// <summary>
        /// Finds a region by position
        /// </summary>
        public MailMergeRegionInfo FindByPosition(int position)
        {
            lock (_lockObject)
            {
                if (_isIndexDirty) RebuildIndexes();

                if (_positionIndex.TryGetValue(position, out var region))
                {
                    return region;
                }

                // Fallback to linear search
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
        }

        /// <summary>
        /// Finds regions that contain a specific position
        /// </summary>
        public IReadOnlyList<MailMergeRegionInfo> FindRegionsContainingPosition(int position)
        {
            lock (_lockObject)
            {
                return _regions.Where(r =>
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
                }).ToList();
            }
        }

        /// <summary>
        /// Finds regions within a specific range
        /// </summary>
        public IReadOnlyList<MailMergeRegionInfo> FindRegionsInRange(int startPosition, int endPosition)
        {
            lock (_lockObject)
            {
                return _regions.Where(r =>
                {
                    try
                    {
                        if (r.StartField == null || r.EndField == null)
                            return false;

                        int regionStart = r.StartField.Range.Start.ToInt();
                        int regionEnd = r.EndField.Range.End.ToInt();

                        return regionStart >= startPosition && regionEnd <= endPosition;
                    }
                    catch { return false; }
                }).ToList();
            }
        }

        public bool IsRegionInRange(MailMergeRegionInfo region, int targetStart, int targetEnd, RegionSearchMode mode)
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

        /// <summary>
        /// Gets all region names
        /// </summary>
        public IReadOnlyList<string> GetAllRegionNames()
        {
            lock (_lockObject)
            {
                return _regions
                    .Where(r => !string.IsNullOrEmpty(r.Name))
                    .Select(r => r.Name)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
        }

        /// <summary>
        /// Gets regions grouped by name
        /// </summary>
        public Dictionary<string, List<MailMergeRegionInfo>> GroupByName()
        {
            lock (_lockObject)
            {
                return _regions
                        .Where(r => !string.IsNullOrEmpty(r.Name))
                        .GroupBy(r => r.Name, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            g => g.Key,
                            g => g.ToList(),
                            StringComparer.OrdinalIgnoreCase);
            }
        }

        /// <summary>
        /// Checks if a region with the specified name exists
        /// </summary>
        public bool ContainsName(string regionName)
        {
            return FindByName(regionName) != null;
        }

        /// <summary>
        /// Gets regions by parent
        /// </summary>
        public IReadOnlyList<MailMergeRegionInfo> GetRegionsByParent(MailMergeRegionInfo parentRegion)
        {
            if (parentRegion == null)
                return new List<MailMergeRegionInfo>();

            lock (_lockObject)
            {
                return _regions.Where(r => r.ParentRegion == parentRegion).ToList();
            }
        }

        /// <summary>
        /// Gets root regions (regions with no parent)
        /// </summary>
        public IReadOnlyList<MailMergeRegionInfo> GetRootRegions()
        {
            lock (_lockObject)
            {
                return _regions.Where(r => r.ParentRegion == null).ToList();
            }
        }

        public IReadOnlyList<MailMergeRegionInfo> GetRegionsByDepth(int depth)
        {
            if (depth < 0)
                throw new ArgumentOutOfRangeException(nameof(depth), "Depth cannot be negative.");

            var allRegions = _regions;
            var regionsAtDepth = new List<MailMergeRegionInfo>();

            foreach (var region in allRegions)
            {
                int currentDepth = 0;
                var current = region;
                while (current.ParentRegion != null)
                {
                    currentDepth++;
                    current = current.ParentRegion;
                }
                if (currentDepth == depth)
                {
                    regionsAtDepth.Add(region);
                }
            }
            return regionsAtDepth;
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

        public IReadOnlyList<MailMergeRegionInfo> GetNestedRegions(MailMergeRegionInfo parentRegion)
        {
            if (parentRegion == null)
                throw new ArgumentNullException(nameof(parentRegion));

            var nestedRegions = new List<MailMergeRegionInfo>();
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

        public IReadOnlyList<MailMergeRegionInfo> GetParentRegions(MailMergeRegionInfo region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));

            var parents = new List<MailMergeRegionInfo>();
            var current = region.ParentRegion;
            while (current != null)
            {
                parents.Add(current);
                current = current.ParentRegion;
            }
            return parents;
        }

        public IReadOnlyList<MailMergeRegionInfo> GetSiblingRegions(MailMergeRegionInfo region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region));
            var siblings = new List<MailMergeRegionInfo>();
            var parent = region.ParentRegion;
            if (parent != null && parent.Regions != null)
            {
                foreach (var sibling in parent.Regions)
                {
                    if (sibling != region)
                    {
                        siblings.Add(sibling);
                    }
                }
            }
            return siblings;
        }

        /// <summary>
        /// Gets regions with merge fields
        /// </summary>
        public IReadOnlyList<MailMergeRegionInfo> GetRegionsWithMergeFields()
        {
            lock (_lockObject)
            {
                return _regions.Where(r => r.MergeFields != null && r.MergeFields.Count > 0).ToList();
            }
        }

        /// <summary>
        /// Gets regions without merge fields
        /// </summary>
        public IReadOnlyList<MailMergeRegionInfo> GetRegionsWithoutMergeFields()
        {
            lock (_lockObject)
            {
                return _regions.Where(r => r.MergeFields == null || r.MergeFields.Count == 0).ToList();
            }
        }

        #endregion

        #region IList Implementation

        public void Add(MailMergeRegionInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_lockObject)
            {
                _regions.Add(item);
                AddToIndexes(item);
                _isIndexDirty = false;
            }
        }

        public void AddRange(IEnumerable<MailMergeRegionInfo> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            lock (_lockObject)
            {
                foreach (var item in items)
                {
                    if (item != null)
                    {
                        _regions.Add(item);
                        AddToIndexes(item);
                    }
                }
                _isIndexDirty = false;
            }
        }

        public void Insert(int index, MailMergeRegionInfo item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_lockObject)
            {
                _regions.Insert(index, item);
                AddToIndexes(item);
                _isIndexDirty = false;
            }
        }

        public bool Remove(MailMergeRegionInfo item)
        {
            if (item == null)
                return false;

            lock (_lockObject)
            {
                bool removed = _regions.Remove(item);
                if (removed)
                {
                    RemoveFromIndexes(item);
                    _isIndexDirty = false;
                }
                return removed;
            }
        }

        public void RemoveAt(int index)
        {
            lock (_lockObject)
            {
                if (index >= 0 && index < _regions.Count)
                {
                    var item = _regions[index];
                    _regions.RemoveAt(index);
                    if (item != null)
                    {
                        RemoveFromIndexes(item);
                    }
                    _isIndexDirty = false;
                }
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _regions.Clear();
                _nameIndex.Clear();
                _positionIndex.Clear();
                _isIndexDirty = false;
            }
        }

        public bool Contains(MailMergeRegionInfo item)
        {
            return _regions.Contains(item);
        }

        public void CopyTo(MailMergeRegionInfo[] array, int arrayIndex)
        {
            _regions.CopyTo(array, arrayIndex);
        }

        public int IndexOf(MailMergeRegionInfo item)
        {
            return _regions.IndexOf(item);
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<MailMergeRegionInfo> GetEnumerator()
        {
            return _regions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _regions.GetEnumerator();
        }

        #endregion

        #region Additional Utility Methods

        /// <summary>
        /// Creates a deep copy of the collection
        /// </summary>
        public MailMergeRegionCollection Clone()
        {
            lock (_lockObject)
            {
                return new (_regions);
            }
        }

        /// <summary>
        /// Filters regions by a predicate
        /// </summary>
        public MailMergeRegionCollection Filter(Func<MailMergeRegionInfo, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            lock (_lockObject)
            {
                var filtered = _regions.Where(predicate).ToList();
                return [.. filtered];
            }
        }

        /// <summary>
        /// Gets regions that are valid (have both start and end fields)
        /// </summary>
        public MailMergeRegionCollection GetValidRegions()
        {
            return Filter(r => r.StartField != null && r.EndField != null);
        }

        /// <summary>
        /// Gets regions that are invalid (missing start or end fields)
        /// </summary>
        public MailMergeRegionCollection GetInvalidRegions()
        {
            return Filter(r => r.StartField == null || r.EndField == null);
        }

        /// <summary>
        /// Gets regions sorted by position
        /// </summary>
        public MailMergeRegionCollection SortByPosition()
        {
            lock (_lockObject)
            {
                var sorted = _regions
                    .Where(r => r.StartField != null)
                    .OrderBy(r => r.StartField.Range.Start.ToInt())
                    .ToList();

                // Add regions without start fields at the end
                var noStartField = _regions
                    .Where(r => r.StartField == null)
                    .ToList();

                sorted.AddRange(noStartField);
                return new(sorted);
            }
        }

        /// <summary>
        /// Gets regions by nesting level
        /// </summary>
        public Dictionary<int, List<MailMergeRegionInfo>> GroupByNestingLevel()
        {
            lock (_lockObject)
            {
                return _regions
                    .GroupBy(r => GetNestingLevel(r))
                    .ToDictionary(
                        g => g.Key,
                        g => g.ToList());
            }
        }

        private int GetNestingLevel(MailMergeRegionInfo region)
        {
            int level = 0;
            var current = region;
            while (current.ParentRegion != null)
            {
                level++;
                current = current.ParentRegion;
            }
            return level;
        }

        /// <summary>
        /// Gets the maximum nesting depth
        /// </summary>
        public int GetMaxNestingDepth()
        {
            lock (_lockObject)
            {
                if (_regions.Count == 0)
                    return 0;

                return _regions.Max(r => GetNestingLevel(r));
            }
        }

        #endregion
    }
}
