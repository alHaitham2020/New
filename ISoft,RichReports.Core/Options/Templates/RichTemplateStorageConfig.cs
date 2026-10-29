using ISoft.RichReports.Core.Abstractions.Storage.Templates;
using ISoft.RichReports.Core.Templates;
using System.IO;

namespace ISoft.RichReports.Core.Options.Templates
{
    /// <summary>
    /// Configuration for template storage locations
    /// </summary>
    public class RichTemplateStorageConfig : IRichTemplateStorageConfig
    {
        /// <summary>
        /// Base path for all templates
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// Subfolder for default templates
        /// </summary>
        public string TemplatesFolder { get; set; } = "Templates";

        /// <summary>
        /// File extension for template files
        /// </summary>
        public string FileExtension { get; set; } = RichReportTemplate.FileExtension;

        /// <summary>
        /// Gets the full default templates path
        /// </summary>
        public string GetTemplatesPath() =>
            Path.Combine(BasePath, TemplatesFolder);
    }
}
