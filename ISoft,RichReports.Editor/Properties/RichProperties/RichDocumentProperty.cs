using ISoft_RichReports.Editor.Fields.Formatting;
using System;

namespace ISoft_RichReports.Editor.Properties.RichProperties
{
    public interface IRichDocumentProperty : IRichProperty
    {
        RichDocumentPropertyId Id { get; }
    }

    // hold the definition for the property
    public class RichDocumentProperty : RichPropertyBase, IRichDocumentProperty
    {
        private readonly RichDocumentPropertyId _id;

        public RichDocumentPropertyId Id => _id;

        public RichDocumentProperty(
            RichDocumentPropertyId id,
            string name,
            string displayName,
            object defaultValue,
            Type propertyType,
            RichFieldFormat? format = null,
            bool? visible = null) 
            : base(name, displayName, defaultValue, propertyType, format, visible)
        {
            _id = id;
        }
    }
}
