using ISoft.RichReports.Core.Abstractions.Templates;
using ISoft.RichReports.Core.Templates.Serialization;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Templates.ImportExport
{
    public static class RichReportTemplateImportExportHelper
    {
        public async static Task<bool> ShowExportDialogAsync(IRichReportTemplate template)
        {
            if (TryShowExportDialog(template, out string fName))
            {
                return await RichReportTemplateSerializationHelper.SerializeToFileAsync(template, fName);
            }

            return false;
        }

        public static bool ShowExportDialog(IRichReportTemplate template)
        {
            if (TryShowExportDialog(template, out string fName))
            {
                RichReportTemplateSerializationHelper.SerializeToFile(template, fName);
                return true;
            }

            return false;
        }

        public static RichReportTemplate ShowImportDialog()
        {
            if (TryShowImportDialog(out var template))
            {
                return template;
            }
            return null;
        }

        public static async Task<RichReportTemplate> ShowImportDialogAsync()
        {
            if (TryShowImportDialogCore(out var fileName))
            {
                return await RichReportTemplateSerializationHelper.DeserializeFromFileAsync(fileName);
            }
            return null;
        }

        public static bool TryShowImportDialog(out RichReportTemplate template)
        {
            if (TryShowImportDialogCore(out string fileName))
            {
                template = RichReportTemplateSerializationHelper.DeserializeFromFile(fileName);
                return true;
            }

            template = null;
            return false;
        }

        private static bool TryShowImportDialogCore(out string fileName)
        {
            return StorageFileDialogHelper.TryShowFileDialog(dialogType: StorageFileDialogType.Open,
                                                             title: $"إستيراد تصميم",
                                                             customFilter: RichReportTemplate.FileFilter,
                                                             defaultFileName: null,
                                                             resultFileName: out fileName);
        }

        private static bool TryShowExportDialog(IRichReportTemplate template, out string fileName)
        {
            return StorageFileDialogHelper.TryShowFileDialog(dialogType: StorageFileDialogType.Save,
                                                             title: $"تصدير تصميم [ {template.GetDisplayName()} ]",
                                                             customFilter: RichReportTemplate.FileFilter,
                                                             defaultFileName: template.Name,
                                                             resultFileName: out fileName);
        }
    }
}
