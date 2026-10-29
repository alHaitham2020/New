using System.ComponentModel;

namespace ISoft.RichReports.Editor.Expressions
{
    public class RichExpressionEvaluationManager
    {
        private readonly PropertyDescriptorCollection _proprties;

        public RichExpressionEvaluationManager(PropertyDescriptorCollection proprties)
        {
            _proprties = proprties;
        }

        public object Evaluate(string expressionString, object dataItem)
        {
            if (string.IsNullOrWhiteSpace(expressionString))
                return null;

            // 3. Parse the criteria expression string into a DevExpress CriteriaOperator
            CriteriaOperator criteria = CriteriaOperator.TryParse(expressionString);
            
            if (criteria.ReferenceEqualsNull()) return null;

            // 4. Create an ExpressionEvaluator using the current record's type descriptors
            //var descriptorCollection = TypeDescriptor.GetProperties(dataItem);
            RichExpressionEvaluator evaluator = new(_proprties, criteria);

            // 5. Evaluate the expression against the current record row
            object evaluationResult = evaluator.Evaluate(dataItem);

            return evaluationResult;
        }
    }

}
