using System.Windows;
using System.Windows.Threading;

namespace ISoft.Wpf.RichReports.Controls
{
    public interface IRichReportDesignerControl : IRichReportDesigner
    {
        internal IBackstageManager<RichReportDesignerBackstageTab> BackstageManager { get; }

        new IXpfRichReport Report { get; }

        IRichReportTemplate ActiveReportTemplate { get; }

        bool AutoUpdateWindowTitle { get; }

        string DefaultNewTemplateName { get; internal set; }

        RichReportDesignerCommands ActualCommands { get; }

        bool IsRightToLeft { get; }

        void ExecuteWithDispatcher(Action action, DispatcherPriority priority);

        Task ExecuteWithDispatcherAsync(Action action, DispatcherPriority priority);

        internal void RaiseActiveTemplateChanged(IRichReportTemplate template);

        internal void RaiseTemplateSaved(IRichReportTemplate template, bool isNew);

        Window GetWindow();
    }
}
