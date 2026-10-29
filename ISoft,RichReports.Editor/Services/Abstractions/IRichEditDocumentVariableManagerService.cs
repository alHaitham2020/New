using DevExpress.XtraRichEdit;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditDocumentVariableManagerService
    {
        string GetDisplayName(string variableName);

        void Calculate(CalculateDocumentVariableEventArgs e);
    }
}
