using ISoft.RichReports.Core.Abstractions.Storage.Templates;

namespace ISoft.RichReports.Core.Models.Templates
{
    public readonly struct RichTemplateFolderKey : IRichTemplateFolderKey
    {
        public RichTemplateFolderKey(string reportId, string folderName)
        {
            ReportId = reportId;
            FolderName = folderName;
        }

        public string ReportId { get; }

        public string FolderName { get; }
    }
}
