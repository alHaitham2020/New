using DevExpress.Mvvm;
using DevExpress.Utils;
using ISoft.RichReportDesigner.Core.Reports.DataModel;
using ISoft.Utils.DataModel;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;

namespace ISoft.RichReportDesigner.UI.Controls
{
    public class RichReportDesignerCommands : ObservableObject
    {
        private enum DesignerCommandId
        {
            NewCommand,
            OpenDefaultCommand,
            OpenCommand,
            SaveCommand,
            SaveAsCommand,
            ShowPreviewCommand,
            ImportCommand,
            ExportCommand,
            RemoveSelectedFieldCommand
        }

        private readonly Dictionary<DesignerCommandId, IDelegateCommand> _commands;

        public RichReportDesignerControl Designer { get; private set; }

        #region Command Properties

        public IDelegateCommand NewCommand => GetCommand(DesignerCommandId.NewCommand);

        public IDelegateCommand OpenDefaultCommand => GetCommand(DesignerCommandId.OpenDefaultCommand);

        public IDelegateCommand OpenCommand => GetCommand(DesignerCommandId.OpenCommand);

        public IDelegateCommand SaveCommand => GetCommand(DesignerCommandId.SaveCommand);

        public IDelegateCommand SaveAsCommand => GetCommand(DesignerCommandId.SaveAsCommand);

        public IDelegateCommand ShowPreviewCommand => GetCommand(DesignerCommandId.ShowPreviewCommand);

        public IDelegateCommand ImportCommand => GetCommand(DesignerCommandId.ImportCommand);

        public IDelegateCommand ExportCommand => GetCommand(DesignerCommandId.ExportCommand);

        #endregion

        public RichReportDesignerCommands(RichReportDesignerControl designer)
        {
            Designer = designer;
            _commands = CreateCommands();
        }

        #region Actions

        private Dictionary<DesignerCommandId, IDelegateCommand> CreateCommands()
        {
            return new()
            {
                [DesignerCommandId.NewCommand] = CommandsFactory.CreateAsyncCommand(Designer.NewAsync, Designer.CanNew),
                [DesignerCommandId.OpenDefaultCommand] = CommandsFactory.CreateAsyncCommand<IRichReportTemplatePreviewItem>(Designer.OpenDefaultAsync, Designer.CanOpenDefault),
                [DesignerCommandId.OpenCommand] = CommandsFactory.CreateAsyncCommand<IRichReportTemplatePreviewItem>(Designer.OpenAsync, Designer.CanOpen),
                [DesignerCommandId.SaveCommand] = CommandsFactory.CreateAsyncCommand(Designer.SaveAsync, Designer.CanSave),
                [DesignerCommandId.SaveAsCommand] = CommandsFactory.CreateAsyncCommand<string>(Designer.SaveAsAsync, Designer.CanSaveAs),
                [DesignerCommandId.ShowPreviewCommand] = CommandsFactory.CreateAsyncCommand(Designer.ShowPrintPreviewAsync, Designer.CanShowPreview),
                [DesignerCommandId.ImportCommand] = CommandsFactory.CreateAsyncCommand(Designer.ImportAsync, Designer.CanImport),
                [DesignerCommandId.ExportCommand] = CommandsFactory.CreateCommand(Designer.Export, Designer.CanExport),
            };
        }

        public void Attach(RichReportDesignerControl designer)
        {
            Guard.ArgumentNotNull(designer, nameof(designer));
            Designer = designer;
            RefreshCommands();
        }

        public void Detach()
        {
            Designer = null;
            RefreshCommands();
        }

        public void RefreshCommands()
        {
            foreach (var command in _commands.Values)
            {
                command.RaiseCanExecuteChanged();
            }
        }

        public void RefreshDesignerCommands()
        {
            SaveCommand.RaiseCanExecuteChanged();
            SaveAsCommand.RaiseCanExecuteChanged();
        }

        protected void AssertAttached()
        {
            if (Designer == null)
            {
                throw new InvalidOperationException();
            }
        }

        private IDelegateCommand GetCommand(DesignerCommandId id) => _commands[id];
        #endregion
    }
}
