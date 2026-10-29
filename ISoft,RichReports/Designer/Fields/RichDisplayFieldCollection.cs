using ISoft.RichReports.Editor.Fields.Core;
using ISoft.Utils.Data.Collection;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Designer.Fields
{
    public class RichDisplayFieldCollection : ObservableUniqueDataCollectionBase<RichEditFieldKey, IRichDisplayField>, IRichDisplayFieldsCollection
    {
        private static readonly Func<IRichDisplayField, RichEditFieldKey> _selector = (x) => x.FieldKey;

        public RichDisplayFieldCollection() : base(_selector)
        {

        }

        public RichDisplayFieldCollection(IEnumerable<IRichDisplayField> collection) : base(collection, _selector)
        {
            
        }

        protected override bool IsValidKey(RichEditFieldKey key) => key.IsValid();

        protected override IRichDisplayField CloneItem(IRichDisplayField item) => item.CreateClone();

        IEnumerable<RichEditFieldKey> IRichDisplayFieldsReadOnlyCollection.FieldKeys => Keys;

        IRichDisplayField IRichDisplayFieldsReadOnlyCollection.FindByFieldProperty(string fieldProperty)
        {
            return Values.SingleOrDefault(x => fieldProperty.EqualsTo(x.FieldProperty));
        }

        bool IRichDisplayFieldsReadOnlyCollection.ContainsField(RichEditFieldKey key) => ContainsKey(key);

        IRichDisplayField IRichDisplayFieldsReadOnlyCollection.GetFieldByKey(RichEditFieldKey key) => GetItem(key);

        bool IRichDisplayFieldsReadOnlyCollection.TryGetField(RichEditFieldKey key, out IRichDisplayField item) => TryGetItem(key, out item);

        void IRichDisplayFieldsCollection.LoadFrom(IEnumerable<IRichDisplayField> source) => LoadFrom(source);
    }
}
