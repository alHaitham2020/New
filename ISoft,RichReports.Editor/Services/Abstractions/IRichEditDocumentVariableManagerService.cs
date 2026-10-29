namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditDocumentVariableManagerService
    {
        string GetDisplayName(string variableName);

        void Calculate(CalculateDocumentVariableEventArgs e);
    }
}
