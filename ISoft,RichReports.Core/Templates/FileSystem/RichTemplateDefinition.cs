using ISoft.Utils.Extentions;
using ISoft.Utils.Resources;
using System;
using System.IO;
using System.Linq;

namespace ISoft.RichReports.Core.Templates.FileSystem
{
    /// <summary>
    /// Template definition for registration
    /// </summary>
    public class RichTemplateDefinition
    {
        /// <summary>
        /// Template file name (without extension)
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Display name shown to users
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Template content (if embedded resource)
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Thumbnail image (optional)
        /// </summary>
        public byte[] Thumbnail { get; set; }

        /// <summary>
        /// Creates a template definition from a file path
        /// </summary>
        public static RichTemplateDefinition FromFile(IResourceFile resourceFile, string displayName = null)
        {
            var path = resourceFile.GetUriString();

            return new RichTemplateDefinition
            {
                FileName = Path.GetFileNameWithoutExtension(path),
                DisplayName = displayName ?? Path.GetFileNameWithoutExtension(path),
                Content = resourceFile.GetStream().CopyAllBytes()
            };
        }

        /// <summary>
        /// Creates a template definition from a file path
        /// </summary>
        public static RichTemplateDefinition FromFile(string filePath, string displayName = null)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Template file not found: {filePath}");

            return new RichTemplateDefinition
            {
                FileName = Path.GetFileNameWithoutExtension(filePath),
                DisplayName = displayName ?? Path.GetFileNameWithoutExtension(filePath),
                Content = File.ReadAllBytes(filePath)
            };
        }

        /// <summary>
        /// Creates a template definition from embedded resource
        /// </summary>
        public static RichTemplateDefinition FromResource(Type assemblyType, string resourceName, string displayName)
        {
            var assembly = assemblyType.Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);

            if (stream == null)
                throw new InvalidOperationException($"Resource not found: {resourceName}");

            var fileName = resourceName.Split('.').Last();
            var content = new byte[stream.Length];
            stream.Read(content, 0, content.Length);

            return new RichTemplateDefinition
            {
                FileName = fileName,
                DisplayName = displayName ?? fileName,
                Content = content
            };
        }
    }
}
