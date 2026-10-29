namespace ISoft.RichReports.Core.Abstractions.Storage.Templates
{
    public interface IRichTemplateFolderKey
    {
        string ReportId { get; }

        string FolderName { get; }
    }
}
