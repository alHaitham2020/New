using ISoft.RichReports.Editor.Fields.Format;
using System;

namespace ISoft.RichReports.Editor.RichProperties
{
    public interface IRichDocumentCustomProperty : IRichPropertyWithValue
    {

    }

    /// <summary>
    /// Represents a custom document property with type-safe value access
    /// </summary>
    public class RichDocumentCustomProperty : RichPropertyWithValueBase, IRichDocumentCustomProperty
    {
        public RichDocumentCustomProperty() { }

        public RichDocumentCustomProperty(
            string name,
            string displayName,
            object value,
            Type propertyType,
            object defaultValue = null,
            RichFieldFormat? format = null,
            bool? visible = null)
            : base(name, displayName, propertyType, value, defaultValue, format, visible)
        {

        }
    }
}
