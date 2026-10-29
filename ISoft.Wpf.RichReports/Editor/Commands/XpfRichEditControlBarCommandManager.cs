using System.Windows.Input;

namespace ISoft.Wpf.RichReports.Editor.Commands
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
