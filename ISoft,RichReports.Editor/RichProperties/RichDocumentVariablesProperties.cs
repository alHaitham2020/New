using System.Collections.Generic;

namespace ISoft.RichReports.Editor.RichProperties
{
    public class RichDocumentVariablesProperties : RichValuePropertiesBase<IRichDocumentVariableProperty>, IRichDocumentVariablesProperties
    {
        public RichDocumentVariablesProperties()
        {

        }

        public RichDocumentVariablesProperties(IEnumerable<IRichDocumentVariableProperty> collection) : base(collection)
        {

        }
    }
}