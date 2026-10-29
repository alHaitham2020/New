using ISoft.Utils.Data.Collection;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public class RichDataFieldGroupCollection : UniqueDataCollectionBase<string, IRichDataFieldGroup>, IRichDataFieldGroupCollection
    {
        private static readonly Func<IRichDataFieldGroup, string> _selector = (x) => x.Path;

        public RichDataFieldGroupCollection() : base(_selector)
        {

        }

        public RichDataFieldGroupCollection(IEnumerable<IRichDataFieldGroup> collection) : base(collection, _selector)
        {
            
        }

        protected override IRichDataFieldGroup CloneItem(IRichDataFieldGroup item)
        {
            return item.CreateClone();
        }

        #region IRichDataFieldGroupCollection
        IEnumerable<string> IRichDataFieldGroupCollection.GroupPaths => Keys;

        bool IRichDataFieldGroupCollection.HasDataGroups => HasItems;

        IRichDataField IRichDataFieldGroupCollection.FindFieldByPath(string fieldPath)
        {
            if (string.IsNullOrEmpty(fieldPath)) return null;

            foreach (IRichDataFieldGroup group in Values)
            {
                if (group.Fields.TryGetField(fieldPath, out var field))
                {
                    return field;
                }
            }

            return null;
        }

        IRichDataFieldGroup IRichDataFieldGroupCollection.FindGroupContainingField(string fieldPath)
        {
            if (string.IsNullOrEmpty(fieldPath)) return null;

            foreach (IRichDataFieldGroup group in Values)
            {
                if (group.Fields.ContainsField(fieldPath))
                {
                    return group;
                }
            }

            return null;
        }

        IRichDataFieldGroup IRichDataFieldGroupCollection.FindGroup(string path) => GetItem(path);

        bool IRichDataFieldGroupCollection.ContainsGroup(string path) => ContainsKey(path);

        bool IRichDataFieldGroupCollection.TryGetGroup(string path, out IRichDataFieldGroup item) => TryGetItem(path, out item);

        void IRichDataFieldGroupCollection.LoadFromSource(IEnumerable<IRichDataFieldGroup> source) => LoadClonedFrom(source);

        #endregion
    }

}
