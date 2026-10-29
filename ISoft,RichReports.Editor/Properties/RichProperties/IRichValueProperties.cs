using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    /// <summary>
    /// value based Proprties
    /// </summary>
    /// <typeparam name="TProperty"></typeparam>
    public interface IRichValueProperties<TProperty> : IRichProperties<TProperty>, IReadOnlyCollection<TProperty> where TProperty : class, IRichPropertyWithValue
    {
        bool RemoveProperty(string name);

        void AddProperty(TProperty item);

        void UpdateProperty(TProperty item);

        void AddOrUpdateProperty(TProperty item);

        void AddProperties(IEnumerable<TProperty> source);

        void LoadFrom(IEnumerable<TProperty> source);
    }
}