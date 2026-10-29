using ISoft.RichReports.Core.Abstractions.Storage.Templates;

namespace ISoft.RichReports.Core.Storage.Templates.FileSystem
{
    public sealed class RichTemplateFolderCache : ConcurrentCacheDictionary<IRichTemplateFolderKey, IRichTemplateFolder>, IRichTemplateFolderCache
    {
        public RichTemplateFolderCache()
        {
           
        }
    }
}
