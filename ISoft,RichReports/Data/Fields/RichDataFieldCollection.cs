using ISoft.Utils.Data.Collection;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public class RichDataFieldCollection : UniqueDataCollectionBase<string, IRichDataField>, IRichDataFieldCollection
    {
        private static readonly Func<IRichDataField, string> _selector = (x) => x.Path;

        public RichDataFieldCollection() : base(_selector)
        {

        }

        public RichDataFieldCollection(IEnumerable<IRichDataField> collection) : base(collection, _selector)
        {

        }

        protected override IRichDataField CloneItem(IRichDataField item)
        {
            return item.CreateClone();
        }

        #region IRichDataFieldCollection
        IEnumerable<string> IRichDataFieldReadOnlyCollection.Paths => Keys;

        bool IRichDataFieldReadOnlyCollection.ContainsField(string path) => ContainsKey(path);

        IRichDataField IRichDataFieldReadOnlyCollection.GetFieldByPath(string path) => GetItem(path);

        string IRichDataFieldReadOnlyCollection.GetFieldPath(IRichDataField item) => GetItemKey(item);

        bool IRichDataFieldReadOnlyCollection.TryGetField(string path, out IRichDataField item) => TryGetItem(path, out item);

        void IRichDataFieldCollection.LoadFrom(IEnumerable<IRichDataField> source) => LoadFrom(source);

        #endregion
    }

}
