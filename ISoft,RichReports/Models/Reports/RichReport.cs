using ISoft.RichReports.Context;

namespace ISoft.RichReports.Models.Reports
{
    /// <summary>
    /// a rich report for all platform
    /// </summary>
    public class RichReport : RichReportBase
    {
        #region Constructors
        public RichReport(IRichReportContext context) : base(context)
        {
        }
        #endregion

        #region Proprties
        #endregion

        #region Actions

        protected override void RegisterServices()
        {
            base.RegisterServices();
            AddService(typeof(IRichWaitIndicationService), new RichDefaultWaitIndicationService());
            AddService(typeof(IRichEditDocumentPrintingService), new RichDefaultEditDocumentPrintingService());
        }

        protected override bool HandelResult(DataValidationResult validationResult)
        {
            return validationResult.IsValid;
        }

        #endregion
    }
}
