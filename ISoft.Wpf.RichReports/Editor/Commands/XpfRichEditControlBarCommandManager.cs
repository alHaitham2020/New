using DevExpress.Utils.Commands;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using System.Windows.Input;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class XpfRichEditControlBarCommandManager : RichEditControlBarCommandManager
    {
        public XpfRichEditControlBarCommandManager(RichEditControl control) : base(control)
        {
            
        }

        internal void ExecuteCommand(RichEditCommandId commandId, object parameter)
        {
            ExecuteParametrizedCommand(commandId, parameter);
        }

        internal void ExecuteCommand(ICommand barItemCommand, object parameter)
        {
            ExecuteParametrizedCommand(barItemCommand, parameter);
        }

        internal void ExecuteCommand(Command command, object parameter)
        {
            ExecuteParametrizedCommand(command, parameter);
        }
    }
}
