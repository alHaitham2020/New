namespace ISoft.RichReports.Core.Abstractions.Storage.Templates
{
    public interface IRichTemplateStorageConfig
    {
        string BasePath { get;}

        string TemplatesFolder { get;}

        string FileExtension { get; }

        string GetTemplatesPath();
    }
}
