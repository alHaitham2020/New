using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    public class RichCalculatedFieldProperties : UniqueDataCollectionBase<string, IRichCalculatedFieldProperty>, IRichCalculatedFieldProperties
    {
        private static readonly Func<IRichCalculatedFieldProperty, string> _keySelector = (x) => x.Name;

        public RichCalculatedFieldProperties() : base(_keySelector)
        {

        }

        public RichCalculatedFieldProperties(IEnumerable<IRichCalculatedFieldProperty> collection) : base(collection, _keySelector)
        {

        }

        IEnumerable<string> IRichCalculatedFieldProperties.Names => Keys;

        bool IRichCalculatedFieldProperties.HasProprties => HasItems;

        bool IRichCalculatedFieldProperties.Contains(string name) => ContainsKey(name);

        IRichCalculatedFieldProperty IRichCalculatedFieldProperties.FindByName(string name) => GetItem(name);

        void IRichCalculatedFieldProperties.LoadFromSource(IEnumerable<IRichCalculatedFieldProperty> source) => LoadFrom(source);

        bool IRichCalculatedFieldProperties.TryGet(string name, out IRichCalculatedFieldProperty item) => TryGetItem(name, out item);
    }
}
