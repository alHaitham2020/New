using ISoft.Mvvm.Extentions;
using ISoft.Mvvm.ViewModel.Common;
using ISoft.RichReportDesigner.UI.DataModel;
using ISoft.RichReportDesigner.UI.UIServices;
using ISoft.Utils.Extentions;

namespace ISoft.RichReportDesigner.UI.ViewModel
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
