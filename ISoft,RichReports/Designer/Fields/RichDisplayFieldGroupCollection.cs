using ISoft.RichReports.Fields;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ISoft.RichReports.Designer.Fields
{
    //public class RichDisplayFieldGroupCollection : ObservableUniqueDataCollectionBase<string, IRichDisplayFieldGroup>, IRichDisplayFieldGroupCollection
    //{
    //    private static readonly Func<IRichDisplayFieldGroup, string> _selector = x => x.Path;

    //    // speedup the search

    //    private IReadOnlyDictionary<RichFieldKey, IRichDisplayField> _allFields;

    //    public RichDisplayFieldGroupCollection() : base(_selector) { }

    //    public RichDisplayFieldGroupCollection(IEnumerable<IRichDisplayFieldGroup> collection) : base(collection, _selector) { }

    //    protected override IRichDisplayFieldGroup CloneItem(IRichDisplayFieldGroup item)
    //        => item.GetClone();

    //    private void Pobulate()
    //    {
    //        _allFields = Values.SelectMany(x => x.Fields).Where(x => x.FieldKey.IsValid()).ToDictionary(x => x.FieldKey);
    //    }

    //    #region IRichDisplayFieldGroupCollection
    //    IEnumerable<string> IRichDisplayFieldGroupReadOnlyCollection.GroupPaths => Keys;

    //    IRichDisplayField IRichDisplayFieldGroupReadOnlyCollection.FindFieldByPath(string fieldPath)
    //    {
    //        if (string.IsNullOrEmpty(fieldPath)) return null;

    //        foreach (var group in Values)
    //        {
    //            if (group.Fields.TryGetField(fieldPath, out var field))
    //                return field;
    //        }
    //        return null;
    //    }

    //    IRichDisplayField IRichDisplayFieldGroupReadOnlyCollection.FindFieldByKey(RichFieldKey fieldKey)
    //    {
    //        return _allFields.TryGetValue(fieldKey, out var field) ? field : null;

    //        //return Values
    //        //    .SelectMany(group => group.Fields)
    //        //    .FirstOrDefault(f => f.IsMatching(fieldKey));
    //    }

    //    IRichDisplayField IRichDisplayFieldGroupReadOnlyCollection.FindNoneDataFieldByPath(string fieldPath)
    //    {
    //        if (string.IsNullOrEmpty(fieldPath)) return null;

    //        foreach (var group in Values.Where(x => x.Category != RichDisplayFieldGroupCategory.DataFields))
    //        {
    //            if (group.Fields.TryGetField(fieldPath, out var field))
    //                return field;
    //        }
    //        return null;
    //    }

    //    IRichDisplayField IRichDisplayFieldGroupReadOnlyCollection.FindDataFieldByPath(string fieldPath)
    //    {
    //        if (string.IsNullOrEmpty(fieldPath)) return null;

    //        foreach (var group in Values.Where(x => x.Category == RichDisplayFieldGroupCategory.DataFields))
    //        {
    //            if (group.Fields.TryGetField(fieldPath, out var field))
    //                return field;
    //        }
    //        return null;
    //    }

    //    IRichDisplayFieldGroup IRichDisplayFieldGroupReadOnlyCollection.FindDataFieldParentGroup(string fieldPath)
    //    {
    //        if (string.IsNullOrEmpty(fieldPath)) return null;

    //        return GetGroupsIn(RichDisplayFieldGroupCategory.DataFields)
    //            .SingleOrDefault(x => x.Fields.ContainsField(fieldPath));
    //    }

    //    IRichDisplayFieldGroup IRichDisplayFieldGroupReadOnlyCollection.FindGroupContainingField(string fieldPath)
    //    {
    //        if (string.IsNullOrEmpty(fieldPath)) return null;

    //        return Values.FirstOrDefault(group => group.Fields.ContainsField(fieldPath));
    //    }

    //    public IRichDisplayFieldGroup FindGroupByCategory(RichDisplayFieldGroupCategory category)
    //    {
    //        if (category == RichDisplayFieldGroupCategory.DataFields)
    //            throw new InvalidOperationException("Data category can return multiple groups.");

    //        return Values.SingleOrDefault(x => x.Category == category);
    //    }

    //    public IRichDisplayFieldGroupReadOnlyCollection GetDataFieldGroups()
    //        => GetGroupsIn(RichDisplayFieldGroupCategory.DataFields);

    //    public IRichDisplayFieldGroupReadOnlyCollection GetNoneDataFieldGroups()
    //        => GetGroupsNotIn(RichDisplayFieldGroupCategory.DataFields);

    //    public IReadOnlyCollection<IRichDisplayField> GetNoneDataFields()
    //    {
    //        return Values
    //            .Where(x => x.Category != RichDisplayFieldGroupCategory.DataFields)
    //            .SelectMany(x => x.Fields)
    //            .ToList();
    //    }

    //    public IReadOnlyCollection<IRichDisplayField> GetAllFields()
    //        => Values.SelectMany(x => x.Fields).ToList();

    //    public IReadOnlyDictionary<RichFieldKey, IRichDisplayField> GetUniqueFields()
    //    {
    //        return _allFields;
    //        //return Values.SelectMany(x => x.Fields).Where(x => x.FieldKey.IsValid()).ToDictionary(x => x.FieldKey);
    //    }

    //    public IRichDisplayFieldGroupReadOnlyCollection GetGroupsIn(params RichDisplayFieldGroupCategory[] categories)
    //    {
    //        if (categories == null) throw new ArgumentNullException(nameof(categories));

    //        RichDisplayFieldGroupCollection result = new();
    //        if (categories.Length == 0) return result;

    //        var categorySet = new HashSet<RichDisplayFieldGroupCategory>(categories);
    //        var groups = Values.Where(i => categorySet.Contains(i.Category)).ToArray();

    //        if (groups.Length > 0)
    //            result.AddRange(groups);

    //        return result;
    //    }

    //    public IRichDisplayFieldGroupReadOnlyCollection GetGroupsNotIn(params RichDisplayFieldGroupCategory[] categories)
    //    {
    //        if (categories == null) throw new ArgumentNullException(nameof(categories));

    //        RichDisplayFieldGroupCollection result = new();
    //        if (categories.Length == 0) return result;

    //        var categorySet = new HashSet<RichDisplayFieldGroupCategory>(categories);
    //        var groups = Values.Where(i => !categorySet.Contains(i.Category)).ToArray();

    //        if (groups.Length > 0)
    //            result.AddRange(groups);

    //        return result;
    //    }

    //    IRichDisplayFieldGroup IRichDisplayFieldGroupReadOnlyCollection.FindGroup(string path) => GetItem(path);

    //    bool IRichDisplayFieldGroupReadOnlyCollection.ContainsGroup(string path) => ContainsKey(path);

    //    bool IRichDisplayFieldGroupReadOnlyCollection.TryGetGroup(string path, out IRichDisplayFieldGroup item) => TryGetItem(path, out item);

    //    void IRichDisplayFieldGroupCollection.LoadFromSource(IEnumerable<IRichDisplayFieldGroup> source)
    //    {
    //        LoadFrom(source);
    //        Pobulate();
    //    }

    //    #endregion
    //}

    public class RichDisplayFieldGroupCollection : ObservableUniqueDataCollectionBase<string, IRichDisplayFieldGroup> , IRichDisplayFieldGroupCollection
    {
        private static readonly Func<IRichDisplayFieldGroup, string> Selector = static x => x.Path;

        // Lazy caches — rebuilt on demand after mutations
        private FrozenDictionary<RichEditFieldKey, IRichDisplayField> _allFields;
        private FrozenDictionary<string, IRichDisplayField> _fieldsByPath;
        private bool _cacheValid;

        public RichDisplayFieldGroupCollection() : base(Selector) { }

        public RichDisplayFieldGroupCollection(IEnumerable<IRichDisplayFieldGroup> collection)
            : base(collection, Selector) { }

        protected override IRichDisplayFieldGroup CloneItem(IRichDisplayFieldGroup item)
            => item.CreateClone();

        #region Cache Building

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureCache()
        {
            if (_cacheValid) return;

            // Build all three lookups in a single pass where possible
            var allFields = new Dictionary<RichEditFieldKey, IRichDisplayField>();
            var fieldsByPath = new Dictionary<string, IRichDisplayField>(StringComparer.OrdinalIgnoreCase);

            foreach (var group in Values)
            {
                foreach (var field in group.Fields)
                {
                    if (field.FieldKey.IsValid())
                    {
                        allFields.Add(field.FieldKey, field);
                    }

                    if (!string.IsNullOrEmpty(field.Path))
                    {
                        fieldsByPath.Add(field.Path, field);
                    }
                }
            }

            _allFields = allFields.ToFrozenDictionary();
            _fieldsByPath = fieldsByPath.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
            _cacheValid = true;
        }

        #endregion

        #region IRichDisplayFieldGroupCollection

        public void InvalidateCache()
        {
            _cacheValid = false;
        }

        public IEnumerable<string> GroupPaths => Keys;

        public IRichDisplayField FindFieldByPath(string fieldPath)
        {
            if (string.IsNullOrEmpty(fieldPath)) return null;

            EnsureCache();

            return _fieldsByPath!.TryGetValue(fieldPath, out var field) ? field : null;
        }

        public IRichDisplayField FindFieldByKey(RichEditFieldKey fieldKey)
        {
            if(!fieldKey.IsValid()) return null;

            EnsureCache();

            return _allFields!.TryGetValue(fieldKey, out var field) ? field : null;
        }

        public IRichDisplayField FindNoneDataFieldByKey(RichEditFieldKey fieldKey)
            => FindFieldByKeyInCategory(fieldKey, RichFieldGroupCategory.DataFields, negate: true);

        public IRichDisplayField FindDataFieldByKey(RichEditFieldKey fieldKey)
            => FindFieldByKeyInCategory(fieldKey, RichFieldGroupCategory.DataFields, negate: false);

        private IRichDisplayField FindFieldByKeyInCategory(
            RichEditFieldKey fieldKey,
            RichFieldGroupCategory category,
            bool negate)
        {
            if (!fieldKey.IsValid()) return null;

            foreach (var group in Values)
            {
                if (negate == (group.Category == category)) continue;
                if (group.Fields.TryGetField(fieldKey, out var field))
                    return field;
            }
            return null;
        }

        public IRichDisplayField FindDataFieldByPath(string fieldPath)
        {
            if (string.IsNullOrEmpty(fieldPath)) return null;

            EnsureCache();

            return _fieldsByPath!.TryGetValue(fieldPath, out var field)
                   && field.FieldType == RichEditFieldType.MergeField
                ? field
                : null;
        }

        public IRichDisplayFieldGroup FindGroupContainingField(RichEditFieldKey fieldKey)
        {
            if (!fieldKey.IsValid()) return null;

            EnsureCache();

            return _allFields!.TryGetValue(fieldKey, out var group) ? group?.ParentGroup : null;
        }

        public IRichDisplayFieldGroup FindGroupByCategory(RichFieldGroupCategory category)
        {
            if (category == RichFieldGroupCategory.DataFields)
                throw new InvalidOperationException("Data category can return multiple groups.");

            return Values.SingleOrDefault(x => x.Category == category);
        }

        public IRichDisplayFieldGroupReadOnlyCollection GetDataFieldGroups()
            => GetGroupsIn(RichFieldGroupCategory.DataFields);

        public IRichDisplayFieldGroupReadOnlyCollection GetNoneDataFieldGroups()
            => GetGroupsNotIn(RichFieldGroupCategory.DataFields);

        public IReadOnlyCollection<IRichDisplayField> GetNoneDataFields()
            => Values
                .Where(static x => x.Category != RichFieldGroupCategory.DataFields)
                .SelectMany(static x => x.Fields)
                .ToArray();

        public IReadOnlyCollection<IRichDisplayField> GetAllFields()
        {
            EnsureCache();
            return _allFields.Values;
        }

        public IRichDisplayFieldGroupReadOnlyCollection GetGroupsIn(params RichFieldGroupCategory[] categories)
        {
            ArgumentGuardHelper.ArgumentNotNull(categories);
            if (categories.Length == 0) return new RichDisplayFieldGroupCollection();

            var set = categories.ToHashSet();
            return FilterGroups(g => set.Contains(g.Category));
        }

        public IRichDisplayFieldGroupReadOnlyCollection GetGroupsNotIn(params RichFieldGroupCategory[] categories)
        {
            ArgumentGuardHelper.ArgumentNotNull(categories);
            if (categories.Length == 0) return new RichDisplayFieldGroupCollection();

            var set = categories.ToHashSet();
            return FilterGroups(g => !set.Contains(g.Category));
        }

        private IRichDisplayFieldGroupReadOnlyCollection FilterGroups(Func<IRichDisplayFieldGroup, bool> predicate)
        {
            var result = new RichDisplayFieldGroupCollection();
            var groups = Values.Where(predicate).ToArray();
            if (groups.Length > 0) result.AddRange(groups);
            return result;
        }

        public IRichDisplayFieldGroup FindGroup(string path)
            => GetItem(path);

        public bool ContainsGroup(string path)
            => ContainsKey(path);

        public bool TryGetGroup(
            string path,
            out IRichDisplayFieldGroup item)
            => TryGetItem(path, out item);

        public void LoadFromSource(IEnumerable<IRichDisplayFieldGroup> source)
        {
            LoadFrom(source);
            _cacheValid = false;
        }

        #endregion
    }

}
