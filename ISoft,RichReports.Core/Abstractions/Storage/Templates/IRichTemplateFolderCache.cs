using ISoft.Utils.Caching;

namespace ISoft.RichReports.Core.Templates.FileSystem
{
    public interface IRichTemplateFolderCache : IConcurrentCacheDictionary<IRichTemplateFolderKey, IRichTemplateFolder>
    {
        
    }
}
