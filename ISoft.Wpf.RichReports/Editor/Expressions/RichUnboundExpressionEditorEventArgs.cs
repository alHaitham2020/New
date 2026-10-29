using DevExpress.Data;
using DevExpress.Xpf.Editors.ExpressionEditor;
using System;
using System.Windows;

namespace ISoft.RichReportDesigner.UI.RichEdit.ExpressionEditor
{
    public delegate void RichUnboundExpressionEditorEventHandler(object sender, RichUnboundExpressionEditorEventArgs e);

    public class RichUnboundExpressionEditorEventArgs : ExpressionEditorCreatedEventArgsBase
    {
        internal IAutoCompleteExpressionEditor _autoCompleteExpressionEditor => base.ActualAutocompleteEditor;

        internal ExpressionEditorControl _expressionEditorControl => base.ActualStandardEditor;

        internal ExpressionEditorMode ActualMode => GetActualMode();

        internal RichUnboundExpressionEditorEventArgs(RoutedEvent routedEvent, IDataColumnInfo column)
            : base(routedEvent, column)
        {

        }

        public RichUnboundExpressionEditorEventArgs(ExpressionEditorControl control, IDataColumnInfo column)
            : base(control, column)
        {
        }

        protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
        {
            ((RichUnboundExpressionEditorEventHandler)genericHandler)(genericTarget, this);
        }
    }

}
