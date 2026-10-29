using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    public interface IRichProperties<TProperty> where TProperty : class, IRichProperty
    {
        TProperty this[string name] { get; }

        IEnumerable<string> Names { get; }

        int Count { get; }

        bool HasProprties { get; }

        bool Contains(string name);

        TProperty GetProperty(string name);

        bool TryGetProperty(string name, out TProperty item);

        IEnumerable<TProperty> GetVisibleProperties();

        bool HasVisibleProprties();
    }
}