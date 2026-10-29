using DevExpress.Data.Controls.ExpressionEditor;
using DevExpress.DataAccess.Native.ExpressionEditor;

namespace ISoft.RichReportDesigner.UI.RichEdit.ExpressionEditor
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
