using ISoft.RichReports.Editor.Fields.Format;
using System;

namespace ISoft.RichReports.Editor.RichProperties
{
    public class RichCalculatedFieldProperty : RichPropertyBase, IRichCalculatedFieldProperty
    {
        public RichCalculatedFieldProperty(
            string name,
            string displayName,
            Type propertyType,
            string dataMemeber,
            string expression,
            RichFieldFormat? format = null,
            bool? visible = null) : base(name, displayName, string.Empty, propertyType, format, visible)
        {
            DataMember = dataMemeber;
            Expression = expression;
        }
        
        public string DataMember { get; private set; }

        public string Expression { get; private set; }

        public bool HasValidDataMember => !string.IsNullOrWhiteSpace(DataMember);
    }
}
