using ISoft.RichReports.Core.Abstractions.Storage.Templates;
using ISoft.RichReports.Core.Utilities.Templates;
using System;
using System.Linq;

namespace ISoft.RichReports.Core.Utilities.Templates
{
    public static class RichTemplateRegistryExtensions
    {
        public static IRichTemplateRegistry RegisterFolder(
            this IRichTemplateRegistry registry,
            string reportId,
            string folderName)
        {
            if (string.IsNullOrEmpty(reportId))
                throw new ArgumentNullException(nameof(reportId));
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));

            return registry.RegisterFolder(new RichTemplateFolderKey(reportId, folderName));
        }

        public static IRichTemplateRegistry RegisterFolders(
            this IRichTemplateRegistry registry,
            params (string ReportId, string FolderName)[] reports)
        {
            return registry.RegisterRange(reports.Select(r => new RichTemplateFolderKey(r.ReportId, r.FolderName)).Cast<IRichTemplateFolderKey>().ToArray());
        }

        public static IRichTemplateRegistry AddFile(
            this IRichTemplateRegistry registry,
            string reportId,
            string folderName,
            string fileName)
        {
            if (string.IsNullOrEmpty(reportId))
                throw new ArgumentNullException(nameof(reportId));
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));

            return registry.AddFiles(new RichTemplateFolderKey(reportId, folderName), fileName);

        }

        public static IRichTemplateRegistry AddFiles(
            this IRichTemplateRegistry registry,
            string reportId,
            string folderName,
            params string[] files)
        {
            if (string.IsNullOrEmpty(reportId))
                throw new ArgumentNullException(nameof(reportId));
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException(nameof(folderName));

            return registry.AddFiles(new RichTemplateFolderKey(reportId, folderName), files);

        }

        public static IRichTemplateFolder GetFolder(
            this IRichTemplateRegistry registry,
            string reportId,
            string folderName)
        {
            if (string.IsNullOrEmpty(reportId) || string.IsNullOrEmpty(folderName)) return null;

            return registry.GetFolder(new RichTemplateFolderKey(reportId, folderName));
        }
    }
}
