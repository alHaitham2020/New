using ISoft.RichReports.Models.Reports;

namespace ISoft.RichReports.Services.Editor.Implementations
{
    public class RichDefaultDocumentVariableManagerService : IRichEditDocumentVariableManagerService
    {
        private readonly IRichReport _report;

        public RichDefaultDocumentVariableManagerService(IRichReport report)
        {
            _report = report;
        }

        public string GetDisplayName(string variableName)
        {
            return _report
                     .DocumentVariablesProperties
                     .GetProperty(variableName)?.DisplayName ?? null;
        }

        public virtual void Calculate(CalculateDocumentVariableEventArgs e)
        {
            var property = _report.DocumentVariablesProperties.GetProperty(e.VariableName);
            if (property != null)
            {
                e.Value = property.GetDisplayValue();
                e.Handled = true;
            }
        }
    }
}
