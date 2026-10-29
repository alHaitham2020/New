using ISoft.RichReports.Editor.Fields.Format;
using System;

namespace ISoft.RichReports.Editor.RichProperties
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
