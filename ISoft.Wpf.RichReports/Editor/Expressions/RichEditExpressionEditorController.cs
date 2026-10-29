namespace ISoft.Wpf.RichReports.Editor.Expressions
{
    public class RichEditExpressionEditorController : ExpressionEditorController
    {
        public RichEditExpressionEditorController(
            IIntelliSenseRichEdit richEdit,
            IAutoCompleteWindow autoComplete,
            IExpressionDocumentationControl documentationControl,
            IToolTipProvider toolTipProvider,
            ExpressionEditorContext context) : base(richEdit, autoComplete, documentationControl, toolTipProvider, context)
        {
            
        }
        
    }
}
