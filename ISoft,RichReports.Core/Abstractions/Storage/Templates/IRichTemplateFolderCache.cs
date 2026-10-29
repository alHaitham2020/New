namespace ISoft.RichReports.Core.Abstractions.Storage.Templates
{
    public interface IRichTemplateFolderCache : IConcurrentCacheDictionary<IRichTemplateFolderKey, IRichTemplateFolder>
    {
        
    }
}
