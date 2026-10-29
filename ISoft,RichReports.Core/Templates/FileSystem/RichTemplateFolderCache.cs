using ISoft.Utils.Caching;

namespace ISoft.RichReports.Core.Templates.FileSystem
{
    public sealed class RichTemplateFolderCache : ConcurrentCacheDictionary<IRichTemplateFolderKey, IRichTemplateFolder>, IRichTemplateFolderCache
    {
        public RichTemplateFolderCache()
        {
           
        }
    }
}
