using ISoft.Mvvm.Native;
using ISoft.RichReportDesigner.Core.Designer;
using ISoft.RichReportDesigner.Core.Templates;
using ISoft.RichReportDesigner.UI.DataModel;
using ISoft.RichReportDesigner.UI.Internal;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ISoft.RichReportDesigner.UI.Controls
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
