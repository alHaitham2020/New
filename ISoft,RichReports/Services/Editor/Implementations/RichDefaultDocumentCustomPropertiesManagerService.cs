using ISoft.RichReports.Models.Reports;
using System.Linq;

namespace ISoft.RichReports.Services.Editor.Implementations
{
    public class RichDefaultDocumentCustomPropertiesManagerService : IRichEditDocumentCustomPropertiesManagerService
    {
        private readonly IRichReport _report;

        public RichDefaultDocumentCustomPropertiesManagerService(IRichReport report)
        {
            _report = report;
        }

        public string GetDisplayName(string propertyName)
        {
            return _report
                     .DocumentCustomProperties
                     .GetProperty(propertyName)?.DisplayName ?? string.Empty;
        }

        public virtual void UpdateDocCustomProperties(DocumentCustomProperties docProperties)
        {
            var customProperties = _report.DocumentCustomProperties.ToList();
            foreach(var property in customProperties)
            {
                docProperties[property.Name] = property.GetDisplayValue();
            }
        }
    }
}
