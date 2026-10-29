using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using System.Collections.Generic;
using System.ComponentModel;

namespace ISoft.RichReports.Editor.Expression
{
    public class RichExpressionEvaluator : ExpressionEvaluator
    {
        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria) : base(descriptor, criteria)
        {

        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria) : base(properties, criteria)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, string criteria) : base(properties, criteria)
        {
        }

        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive) : base(descriptor, criteria, caseSensitive)
        {
        }

        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions) : base(descriptor, criteria, customFunctions)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive) : base(properties, criteria, caseSensitive)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive) : base(properties, criteria, caseSensitive)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions) : base(properties, criteria, customFunctions)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, ICollection<ICustomFunctionOperator> customFunctions) : base(properties, criteria, customFunctions)
        {
        }

        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) : base(descriptor, criteria, caseSensitive, customFunctions)
        {
        }

        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, ICollection<ICustomFunctionOperator> customFunctions, ICollection<ICustomAggregate> customAggregates) : base(descriptor, criteria, customFunctions, customAggregates)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) : base(properties, criteria, caseSensitive, customFunctions)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, string criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions) : base(properties, criteria, caseSensitive, customFunctions)
        {
        }

        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions) : base(descriptor, criteria, caseSensitive, doCreateEvaluatorCore, customFunctions)
        {
        }

        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, ICollection<ICustomFunctionOperator> customFunctions, ICollection<ICustomAggregate> customAggregates) : base(descriptor, criteria, caseSensitive, customFunctions, customAggregates)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions) : base(properties, criteria, caseSensitive, doCreateEvaluatorCore, customFunctions)
        {
        }

        public RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions, ICollection<ICustomAggregate> customAggregates) : base(descriptor, criteria, caseSensitive, doCreateEvaluatorCore, customFunctions, customAggregates)
        {
        }

        public RichExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore, ICollection<ICustomFunctionOperator> customFunctions, ICollection<ICustomAggregate> customAggregates) : base(properties, criteria, caseSensitive, doCreateEvaluatorCore, customFunctions, customAggregates)
        {
        }

        protected RichExpressionEvaluator(EvaluatorContextDescriptor descriptor, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore) : base(descriptor, criteria, caseSensitive, doCreateEvaluatorCore)
        {
        }

        protected RichExpressionEvaluator(PropertyDescriptorCollection properties, CriteriaOperator criteria, bool caseSensitive, bool doCreateEvaluatorCore) : base(properties, criteria, caseSensitive, doCreateEvaluatorCore)
        {
        }
    }
}
