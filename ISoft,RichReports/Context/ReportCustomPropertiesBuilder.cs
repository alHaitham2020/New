using ISoft.RichReports.Editor.RichProperties;
using ISoft.RichReports.Localization;

namespace ISoft.RichReports.UI
{
    /// <summary>
    /// Extension methods for adding report properties to custom document properties
    /// </summary>
    public static class ReportCustomPropertiesBuilder
    {
        public static void AddReportProperties(this IRichDocumentCustomProperties customProperties, IRichReportProperties reportProperties)
        {
            if (customProperties is null || reportProperties is null)
                return;

            var properties = new IRichDocumentCustomProperty[5]
            {
                CreateProperty(RichReportStringId.ReportProperties_FileName,nameof(IRichReportProperties.FileName), reportProperties.FileName),
                CreateProperty(RichReportStringId.ReportProperties_ServerName,nameof(IRichReportProperties.ServerName), reportProperties.ServerName),
                CreateProperty(RichReportStringId.ReportProperties_DatabaseName,nameof(IRichReportProperties.DatabaseName), reportProperties.DatabaseName),
                CreateProperty(RichReportStringId.ReportProperties_UserName,nameof(IRichReportProperties.UserName), reportProperties.UserName),
                CreateProperty(RichReportStringId.ReportProperties_AppVersion,nameof(IRichReportProperties.AppVersion), reportProperties.AppVersion)
            };

            customProperties.AddProperties(properties);
        }

        private static RichDocumentCustomProperty CreateProperty(RichReportStringId resourceId, string name, string value)
        {
            var dn = GetLocalizedDisplayName(resourceId);

            return new RichDocumentCustomProperty(
                name: name,
                displayName: dn,
                value: value,
                propertyType: typeof(string));
        }

        private static string GetLocalizedDisplayName(RichReportStringId resourceId)
        {
            return RichReportLocalizer.GetString(resourceId);
        }
    }
}
