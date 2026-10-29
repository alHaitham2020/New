namespace ISoft.RichReports.Services.Editor.Implementations
{
    internal class RichDefaultEditDocumentPrintingService : IRichEditDocumentPrintingService
    {
        public void Export(IRichEditDocumentServer printable, ExportFormat format, string defaultFileName = null)
        {
            ComponentPrinterDynamic printer = new(printable as IBasePrintable, new PrintingSystemBase());

            printer.Export(ExportTarget.Pdf,defaultFileName);
        }

        public void Print(IRichEditDocumentServer printable)
        {
            ComponentPrinterDynamic printer = new(printable as IBasePrintable, new PrintingSystemBase());

            printer.Print();
        }

        public void ShowPrintDialog(IRichEditDocumentServer printable)
        {
            ComponentPrinterDynamic printer = new(printable as IBasePrintable, new PrintingSystemBase());

            printer.PrintDialog();
        }

        public void ShowPrintPreview(IBasePrintable printable, string title = null)
        {
            ComponentPrinterDynamic printer = new(printable, new PrintingSystemBase());

            printer.ShowPreview(null);

        }
    }
}
