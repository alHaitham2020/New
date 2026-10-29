using ISoft.RichReports.Core.Abstractions.Storage.Templates;
using ISoft.RichReports.Core.Abstractions.Templates;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Templates.Factories
{
    /// <summary>
    /// Factory for creating template providers
    /// </summary>
    public static class RichDefaultTemplateInfoProviderFactory
    {
        /// <summary>
        /// Creates a default template provider with a custom registry
        /// </summary>
        public static IRichDefaultTemplateInfoProvider Create(IRichTemplateRegistry registry)
        {
            return new RichRegistryDefaultTemplateProvider(registry);
        }

        /// <summary>
        /// Creates a pre-loaded template provider
        /// </summary>
        public static async Task<IRichDefaultTemplateInfoProvider> CreatePreLoadedAsync(IRichTemplateRegistry registry)
        {
            var provider = Create(registry);

            await provider.PreLoadAllAsync();
            return provider;
        }
    }
}
