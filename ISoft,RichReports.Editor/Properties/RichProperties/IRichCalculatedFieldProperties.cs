using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    public interface IRichCalculatedFieldProperties : IReadOnlyCollection<IRichCalculatedFieldProperty>
    {
        IEnumerable<string> Names { get; }

        bool HasProprties{ get; }

        bool Contains(string name);

        bool TryGet(string name, out IRichCalculatedFieldProperty item);

        IRichCalculatedFieldProperty FindByName(string name);

        internal void LoadFromSource(IEnumerable<IRichCalculatedFieldProperty> source);
    }
}
