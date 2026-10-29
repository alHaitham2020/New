using DevExpress.Data.Filtering;
using DevExpress.XtraReports.Native;

namespace ISoft.RichReports.Editor.Expression
{
    public class RichExpressionFieldNameStringVisitor : FullNameFilterStringVisitor
    {
        //public override string ConvertNameCore(string name)
        //{
        //    return name;
        //}

        // Visit methods for each node type. The generic method is the entry point.
        public object Visit(CriteriaOperator theOperator)
        {
            // Dispatch to the correct Visit method based on the actual runtime type.
            return theOperator?.Accept(this);
        }
    }
}
