using ISoft.RichReports.Models.Reports;

namespace ISoft.RichReports.Services.Editor.Implementations
{
    public class RichEditCalculatedFieldManagerService : IRichEditCalculatedFieldManagerService
    {
        private readonly IRichReport _report;

        public RichEditCalculatedFieldManagerService(IRichReport report)
        {
            _report = report;
        }

        public IRichCalculatedFieldProperties CalculatedFieldProperties => _report.CalculatedFieldProperties;

        public string GetFieldDisplayName(string fieldName)
        {
            return _report.CalculatedFieldProperties.FindByName(fieldName)?.DisplayName ?? string.Empty;
        }

        public string GetFieldExpression(string fieldName)
        {
            return _report.CalculatedFieldProperties.FindByName(fieldName)?.Expression ?? null;
        }

        public bool TryGetFieldExpression(string fieldName, out string expression)
        {
            var result = _report.CalculatedFieldProperties.TryGet(fieldName, out var fieldProperty);
            expression = fieldProperty?.Expression ?? null;

            return result && expression != null;
        }
    }
}
