using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    public class RichDocumentCustomProperties : RichValuePropertiesBase<IRichDocumentCustomProperty>, IRichDocumentCustomProperties
    {
        public RichDocumentCustomProperties()
        {

        }

        public RichDocumentCustomProperties(IEnumerable<IRichDocumentCustomProperty> collection) : base(collection)
        {

        }
    }
}

