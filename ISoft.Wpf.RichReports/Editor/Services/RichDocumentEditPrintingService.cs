namespace ISoft.Wpf.RichReports.Editor.Services
{
    public class RichDocumentEditPrintingService : RichEditPrintingService, IRichEditPrintingService
    {
        void IRichEditPrintingService.ShowPrintPreview(IRichEditControl control)
        {
            control.ShowPrintPreview(rightToLeftLayout: control.DocumentViewDirection == DocumentViewDirection.RightToLeft);
        }
    }
}
