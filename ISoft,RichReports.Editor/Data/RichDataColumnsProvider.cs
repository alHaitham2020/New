using DevExpress.Data;
using DevExpress.Data.Browsing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ISoft.RichReports.Editor.Data
{
    public enum ColumnExpansionMode
    {
        /// <summary>Only expand list-typed properties (master/detail). Default.</summary>
        ListsOnly,

        /// <summary>Expand list-typed and reference-typed properties (full object graph).</summary>
        AllNested
    }

    /// <summary>
    /// Resolves, caches, and traverses IDataColumnInfo trees for a data source
    /// using a DataContextBase.
    /// </summary>
    public sealed class RichDataColumnsProvider : IDisposable
    {
        #region Fields
        private const int DefaultMaxDepth = 5;

        private readonly DataContextBase _context;
        private readonly bool _ownsContext;
        private readonly ColumnExpansionMode _mode;
        private readonly int _maxDepth;

        // Single cache keyed by (source, member). Child expansions are stored
        // in the same dictionary using a (source, member, propertyName) key,
        // so Invalidate/ClearCache handle both consistently.
        private readonly Dictionary<CacheKey, List<IDataColumnInfo>> _cache = new();
        private readonly object _sync = new();
        #endregion
        // =====================================================================
        // CONSTRUCTION
        // =====================================================================

        public RichDataColumnsProvider(
            bool suppressListFilling = false,
            ColumnExpansionMode mode = ColumnExpansionMode.ListsOnly,
            int maxDepth = DefaultMaxDepth)
        {
            _context = new DataContextBase(suppressListFilling);
            _ownsContext = true;
            _mode = mode;
            _maxDepth = maxDepth > 0 ? maxDepth : DefaultMaxDepth;
        }

        public RichDataColumnsProvider(
            DataContextBase context,
            ColumnExpansionMode mode = ColumnExpansionMode.ListsOnly,
            int maxDepth = DefaultMaxDepth)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _ownsContext = false;
            _mode = mode;
            _maxDepth = maxDepth > 0 ? maxDepth : DefaultMaxDepth;
        }

        // =====================================================================
        // PUBLIC API — GET COLUMNS
        // =====================================================================

        public IReadOnlyList<IDataColumnInfo> GetColumns(object dataSource)
            => GetColumns(dataSource, string.Empty);

        public IReadOnlyList<IDataColumnInfo> GetColumns(object dataSource, string dataMember)
        {
            if (dataSource == null) return Array.Empty<IDataColumnInfo>();

            dataMember ??= string.Empty;
            var key = CacheKey.ForRoot(dataSource, dataMember);

            lock (_sync)
            {
                if (_cache.TryGetValue(key, out var cached))
                    return cached;

                var columns = BuildColumns(dataSource, dataMember);
                _cache[key] = columns;
                return columns;
            }
        }

        public IReadOnlyList<IDataColumnInfo> GetColumnsRecursive(object dataSource)
            => GetColumnsRecursive(dataSource, string.Empty);

        public IReadOnlyList<IDataColumnInfo> GetColumnsRecursive(object dataSource, string dataMember)
        {
            var top = GetColumns(dataSource, dataMember);
            var result = new List<IDataColumnInfo>();
            Flatten(top, result);
            return result;
        }

        public IReadOnlyList<RichDataColumnWithPath> GetColumnsWithPath(object dataSource)
            => GetColumnsWithPath(dataSource, string.Empty);

        public IReadOnlyList<RichDataColumnWithPath> GetColumnsWithPath(object dataSource, string dataMember)
        {
            var top = GetColumns(dataSource, dataMember);
            var result = new List<RichDataColumnWithPath>();
            FlattenWithPath(top, string.Empty, result);
            return result;
        }

        public IReadOnlyDictionary<string, IDataColumnInfo> GetLookup(object dataSource)
            => GetLookup(dataSource, string.Empty);

        public IReadOnlyDictionary<string, IDataColumnInfo> GetLookup(object dataSource, string dataMember)
        {
            var dict = new Dictionary<string, IDataColumnInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in GetColumnsWithPath(dataSource, dataMember))
                dict[item.Path] = item.Column;
            return dict;
        }

        public IDataColumnInfo Find(object dataSource, Func<IDataColumnInfo, bool> predicate)
            => Find(dataSource, string.Empty, predicate);

        public IDataColumnInfo Find(object dataSource, string dataMember, Func<IDataColumnInfo, bool> predicate)
        {
            if (predicate == null) return null;
            foreach (var col in GetColumnsRecursive(dataSource, dataMember))
                if (predicate(col)) return col;
            return null;
        }

        public IReadOnlyList<IDataColumnInfo> FindAll(object dataSource, Func<IDataColumnInfo, bool> predicate)
            => FindAll(dataSource, string.Empty, predicate);

        public IReadOnlyList<IDataColumnInfo> FindAll(object dataSource, string dataMember, Func<IDataColumnInfo, bool> predicate)
        {
            if (predicate == null) return Array.Empty<IDataColumnInfo>();
            var result = new List<IDataColumnInfo>();
            foreach (var col in GetColumnsRecursive(dataSource, dataMember))
                if (predicate(col)) result.Add(col);
            return result;
        }

        public string DumpTree(object dataSource)
            => DumpTree(dataSource, string.Empty);

        public string DumpTree(object dataSource, string dataMember)
        {
            var sb = new StringBuilder();
            Write(GetColumns(dataSource, dataMember), sb, 0);
            return sb.ToString();
        }

        // =====================================================================
        // PUBLIC API — CACHE
        // =====================================================================

        public void Invalidate(object dataSource) => Invalidate(dataSource, string.Empty);

        public void Invalidate(object dataSource, string dataMember)
        {
            if (dataSource == null) return;
            dataMember ??= string.Empty;

            lock (_sync)
            {
                // Remove root entry plus every child entry rooted at this source/member.
                var toRemove = _cache.Keys
                    .Where(k => k.MatchesRoot(dataSource, dataMember))
                    .ToList();
                foreach (var k in toRemove) _cache.Remove(k);
            }
        }

        public void ClearCache()
        {
            lock (_sync) _cache.Clear();
        }

        // =====================================================================
        // BUILD
        // =====================================================================

        private List<IDataColumnInfo> BuildColumns(object dataSource, string dataMember)
        {
            var result = new List<IDataColumnInfo>();

            var browser = _context.GetDataBrowser(dataSource, dataMember, suppressException: true);
            var props = browser?.GetItemProperties();
            if (props == null) return result;

            foreach (PropertyDescriptor prop in props)
                result.Add(new RichDataColumnInfo(this, dataSource, dataMember, prop, _mode));

            return result;
        }

        // =====================================================================
        // INTERNAL — EXPANSION
        // =====================================================================

        internal List<IDataColumnInfo> ExpandChildren(
            object dataSource,
            string parentDataMember,
            PropertyDescriptor property,
            ColumnExpansionMode mode,
            HashSet<object> visited)
        {
            if (dataSource == null || property == null)
                return new List<IDataColumnInfo>();

            var key = CacheKey.ForChild(dataSource, parentDataMember, property.Name);

            lock (_sync)
            {
                if (_cache.TryGetValue(key, out var cached))
                    return cached;
            }

            var children = BuildChildren(dataSource, parentDataMember, property, mode, visited);

            lock (_sync) _cache[key] = children;
            return children;
        }

        private List<IDataColumnInfo> BuildChildren(
            object dataSource,
            string parentDataMember,
            PropertyDescriptor property,
            ColumnExpansionMode mode,
            HashSet<object> visited)
        {
            var children = new List<IDataColumnInfo>();

            // Depth guard: count dots in the resolved child member path.
            string childMember = Join(parentDataMember, property.Name);
            if (CountDots(childMember) >= _maxDepth)
                return children;

            bool isList = ListTypeHelper.IsListType(property.PropertyType);
            bool isComplex = DataContext.IsComplexType(property.PropertyType);

            bool shouldExpand = mode switch
            {
                ColumnExpansionMode.AllNested => isList || isComplex,
                _ => isList
            };
            if (!shouldExpand) return children;

            var childBrowser = _context.GetDataBrowser(dataSource, childMember, suppressException: true);
            var childProps = childBrowser?.GetItemProperties();
            if (childProps == null) return children;

            // Cycle guard: hash on the child member path + type identity,
            // not on the (possibly huge / mutating) value graph.
            // This is deterministic and cache-safe.
            var cycleKey = new CycleKey(dataSource, childMember);
            if (!visited.Add(cycleKey))
                return children;

            try
            {
                foreach (PropertyDescriptor childProp in childProps)
                    children.Add(new RichDataColumnInfo(this, dataSource, childMember, childProp, mode, visited));
            }
            finally
            {
                visited.Remove(cycleKey);
            }

            return children;
        }

        internal DataBrowser GetBrowser(object dataSource, string dataMember, bool suppressException)
            => dataSource == null ? null : _context.GetDataBrowser(dataSource, dataMember, suppressException);

        internal string GetColumnName(object dataSource, string dataMember, bool suppressException)
            => _context.GetColumnName(dataSource, dataMember, suppressException);

        internal Type GetPropertyType(object dataSource, string dataMember)
            => _context.GetPropertyType(dataSource, dataMember);

        internal PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember)
            => _context.GetItemProperties(dataSource, dataMember);

        internal string GetListName(object dataSource, string dataMember)
            => _context.GetListName(dataSource, dataMember);

        // =====================================================================
        // FLATTEN / WRITE HELPERS
        // =====================================================================

        private static void Flatten(IEnumerable<IDataColumnInfo> columns, List<IDataColumnInfo> sink)
        {
            if (columns == null) return;
            foreach (var column in columns)
            {
                if (column == null) continue;
                sink.Add(column);
                if (column.Columns != null) Flatten(column.Columns, sink);
            }
        }

        private static void FlattenWithPath(
            IEnumerable<IDataColumnInfo> columns,
            string parentPath,
            List<RichDataColumnWithPath> sink)
        {
            if (columns == null) return;
            foreach (var column in columns)
            {
                if (column == null) continue;

                string path = Join(parentPath, column.Name);

                sink.Add(new RichDataColumnWithPath(column, path));

                if (column.Columns != null)
                    FlattenWithPath(column.Columns, path, sink);
            }
        }

        private static void Write(IEnumerable<IDataColumnInfo> columns, StringBuilder sb, int indent)
        {
            if (columns == null || sb == null) return;

            string pad = new string(' ', indent * 2);
            foreach (var column in columns)
            {
                if (column == null) continue;

                sb.AppendLine(
                    $"{pad}- {column.Caption} [{column.Name}] " +
                    $"Field={column.FieldName}, Type={column.FieldType?.Name}, " +
                    $"Unbound={column.UnboundExpression}");

                if (column.Columns != null)
                    Write(column.Columns, sb, indent + 1);
            }
        }

        private static string Join(string prefix, string member)
            => string.IsNullOrEmpty(prefix) ? member : prefix + "." + member;

        private static int CountDots(string s)
        {
            int n = 0;
            if (s == null) return 0;
            foreach (char c in s) if (c == '.') n++;
            return n;
        }

        // =====================================================================
        // DISPOSE
        // =====================================================================

        public void Dispose()
        {
            ClearCache();
            if (_ownsContext) _context.Dispose();
        }

        // =====================================================================
        // SUPPORT TYPES
        // =====================================================================

        /// <summary>
        /// Cache key for either a root (source, member) or a child
        /// (source, member, propertyName) expansion. Uses weak reference to the
        /// source so we don't keep data sources alive unnecessarily, but caches
        /// the source's hash code so lookups stay stable.
        /// </summary>
        private readonly struct CacheKey : IEquatable<CacheKey>
        {
            private readonly WeakReference _source;
            private readonly int _sourceHash;
            private readonly string _member;        // normalized (lowercase)
            private readonly string _property;      // null for root keys
            private readonly int _hash;

            private CacheKey(object source, string member, string property)
            {
                _source = new WeakReference(source, trackResurrection: false);
                _sourceHash = source?.GetHashCode() ?? 0;
                _member = (member ?? string.Empty).ToLowerInvariant();
                _property = property;
                _hash = HashCode.Combine(_sourceHash, _member, _property ?? string.Empty);
            }

            public static CacheKey ForRoot(object source, string member)
                => new(source, member, null);

            public static CacheKey ForChild(object source, string member, string property)
                => new(source, member, property);

            public bool MatchesRoot(object source, string member)
                => ReferenceEquals(_source.Target, source)
                   && string.Equals(_member, (member ?? string.Empty).ToLowerInvariant(), StringComparison.Ordinal);

            public bool Equals(CacheKey other)
                => _sourceHash == other._sourceHash
                   && string.Equals(_member, other._member, StringComparison.Ordinal)
                   && string.Equals(_property, other._property, StringComparison.Ordinal)
                   && ReferenceEquals(_source.Target, other._source.Target);

            public override bool Equals(object obj) => obj is CacheKey k && Equals(k);

            public override int GetHashCode() => _hash;
        }

        /// <summary>
        /// Cycle-detection key: identity of the data source + the resolved child
        /// member path. Cheaper and more predictable than hashing arbitrary values.
        /// </summary>
        private readonly struct CycleKey : IEquatable<CycleKey>
        {
            private readonly object _source;
            private readonly string _member;

            public CycleKey(object source, string member)
            {
                _source = source;
                _member = member ?? string.Empty;
            }

            public bool Equals(CycleKey other)
                => ReferenceEquals(_source, other._source)
                   && string.Equals(_member, other._member, StringComparison.Ordinal);

            public override bool Equals(object obj) => obj is CycleKey k && Equals(k);

            public override int GetHashCode()
                => HashCode.Combine(RuntimeHelpers.GetHashCode(_source), _member);
        }

        // =====================================================================
        // COLUMN IMPLEMENTATION
        // =====================================================================

        private sealed class RichDataColumnInfo : IDataColumnInfo
        {
            private readonly RichDataColumnsProvider _provider;
            private readonly object _dataSource;
            private readonly string _dataMember;
            private readonly PropertyDescriptor _prop;
            private readonly ColumnExpansionMode _mode;
            private readonly HashSet<object> _visited;

            private List<IDataColumnInfo> _children;

            public RichDataColumnInfo(
                RichDataColumnsProvider provider,
                object dataSource,
                string dataMember,
                PropertyDescriptor prop,
                ColumnExpansionMode mode,
                HashSet<object> visited = null)
            {
                _provider = provider;
                _dataSource = dataSource;
                _dataMember = dataMember ?? string.Empty;
                _prop = prop;
                _mode = mode;
                _visited = visited ?? new HashSet<object>();
            }

            public List<IDataColumnInfo> Columns
                => _children ??= _provider.ExpandChildren(_dataSource, _dataMember, _prop, _mode, _visited);

            public string UnboundExpression => string.Empty;
            public string Caption => _prop.DisplayName;
            public string FieldName => _prop.Name;
            public string Name => _prop.Name;
            public Type FieldType => _prop.PropertyType;
            public DataControllerBase Controller => null;
        }
    }

}
