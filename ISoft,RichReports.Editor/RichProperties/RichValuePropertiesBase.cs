using ISoft.Utils.Data.Collection;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Editor.RichProperties
{
    public abstract class RichValuePropertiesBase<TProperty> : UniqueDataCollectionBase<string, TProperty>, IRichValueProperties<TProperty> where TProperty : class, IRichPropertyWithValue
    {
        private static readonly Func<TProperty, string> _selector = (x) => x.Name;

        public RichValuePropertiesBase() : base(_selector)
        {

        }

        public RichValuePropertiesBase(IEnumerable<TProperty> collection) : base(collection, _selector)
        {

        }

        protected override TProperty CloneItem(TProperty item)
        {
            return item.CreateClone();
        }

        #region IRichValueProperties<TProperty>
        bool IRichProperties<TProperty>.HasProprties => IndexCount > 0;

        IEnumerable<string> IRichProperties<TProperty>.Names => Keys;

        bool IRichProperties<TProperty>.Contains(string name) => ContainsKey(name);

        TProperty IRichProperties<TProperty>.GetProperty(string name) => GetItem(name);

        bool IRichProperties<TProperty>.TryGetProperty(string name, out TProperty property) => TryGetItem(name, out property);

        void IRichValueProperties<TProperty>.AddProperty(TProperty item) => Add(item);

        void IRichValueProperties<TProperty>.UpdateProperty(TProperty item) => TryUpdate(item);

        void IRichValueProperties<TProperty>.AddOrUpdateProperty(TProperty item) => AddOrUpdate(item);

        bool IRichValueProperties<TProperty>.RemoveProperty(string name) => RemoveByKey(name);

        void IRichValueProperties<TProperty>.LoadFrom(IEnumerable<TProperty> source) => LoadFrom(source);

        void IRichValueProperties<TProperty>.AddProperties(IEnumerable<TProperty> source) => AddRange(source);

        IEnumerable<TProperty> IRichProperties<TProperty>.GetVisibleProperties() => Values.Where(p => p.IsVisible);

        bool IRichProperties<TProperty>.HasVisibleProprties() => ((IRichProperties<TProperty>)this).GetVisibleProperties().Any();
        #endregion
    }
}



