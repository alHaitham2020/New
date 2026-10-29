namespace ISoft.RichReports.Core.Templates.FileSystem
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
