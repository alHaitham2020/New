using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Services.Implementation;
using ISoft.RichReportDesigner.Core.Printing;

namespace ISoft.RichReportDesigner.UI.RichEdit.Services
{
    public class RichDocumentEditPrintingService : RichEditPrintingService, IRichEditPrintingService
    {
        void IRichEditPrintingService.ShowPrintPreview(IRichEditControl control)
        {
            control.ShowPrintPreview(rightToLeftLayout: control.DocumentViewDirection == DocumentViewDirection.RightToLeft);
        }
    }
}
