using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    public interface IRichDocumentProperties : IRichProperties<IRichDocumentProperty>
    {
        IEnumerable<IRichDocumentProperty> GetAllProperties();

        IRichDocumentProperty GetProperty(RichDocumentPropertyId id);

        void UpdatePropertyVisiblity(RichDocumentPropertyId id, bool visible);

        T GetPropertyDefaultValue<T>(RichDocumentPropertyId id);

        void ForEachProperty(Action<IRichDocumentProperties, IRichDocumentProperty> action);

        void ForEachPropertyId(Action<IRichDocumentProperties, RichDocumentPropertyId> action);
    }
}
