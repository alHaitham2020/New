using ISoft.RichReports.Models.Reports;
using System;

namespace ISoft.RichReports.Services.Editor.Implementations
{
    public class RichDefaultDocumentPropertiesManagerService : IRichEditDocumentPropertiesManagerService
    {
        private readonly IRichReport _report;

        public RichDefaultDocumentPropertiesManagerService(IRichReport report)
        {
            _report = report;
        }

        public string GetDisplayName(RichDocumentPropertyId id)
        {
            return _report
                     .DocumentProperties
                     .GetProperty(id)?.DisplayName ?? null;
        }

        public string GetDisplayName(string name)
        {
            return _report
                     .DocumentProperties
                     .GetProperty(name)?.DisplayName ?? null;
        }

        public virtual void UpdateDocPreviewProperties(DocumentProperties properties, IRichReportTemplate template)
        {
            properties.Subject = _report.ReportInfo.Name;
            properties.Title = _report.ReportInfo.Title;
            properties.LastPrinted = DateTime.Now;
           
            template.DoIfNotNull(t =>
            {
                properties.Identifier = t.Id.TryConvertToString();
                properties.Template = t.Name;
                properties.Version = t.Version;
            });
        }

        public virtual void UpdateDocDesignProperties(DocumentProperties properties, IRichReportTemplate template)
        {
            properties.Subject = _report.ReportInfo.Name;
            properties.Title = _report.ReportInfo.Title;
            properties.AppVersion = _report.Properties.AppVersion;
            properties.LastModifiedBy = _report.Properties.UserName;
            properties.Category = "ReportTemplate";
            properties.Modified = DateTime.Now;

            template.DoIfNotNull(t =>
            {
                properties.Identifier = t.Id.TryConvertToString();
                properties.Template = t.Name;
                properties.Version = t.Version;
            });
        }
    }
}
