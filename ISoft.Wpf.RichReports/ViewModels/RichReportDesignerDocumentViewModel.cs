using ISoft.Wpf.RichReports.Services.UI;

namespace ISoft.Wpf.RichReports.ViewModels
{
    public class RichReportDesignerDocumentViewModel : DocumentViewModelBase
    {
        #region Propties
        public virtual IXpfRichReport Report { get; protected set; }

        private IRichReportDesignerControlService DesignerService => this.GetLocalRequiredService<IRichReportDesignerControlService>();

        #endregion

        public RichReportDesignerDocumentViewModel(IXpfRichReport report)
        {
            Report = report;
        }

        #region Ovverieds

        protected override bool TryClosing()
        {
            return base.TryClosing() && CanCloseTemplate();
        }

        #endregion

        protected bool CanCloseTemplate() =>
           !DesignerService.NeedsSave() || MessageBoxService.ShowEntityCancelConfirmation();
    }
}
