using ISoft.RichReports.Core.Abstractions.Templates;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Abstractions.Storage.Templates
{
    /// <summary>
    /// Represents a template file in the file system
    /// </summary>
    public interface IRichTemplateFile
    {
        string Name { get; }

        string FullPath { get; }

        IRichTemplateFolder Folder { get; }

        bool Exists { get; }

        bool IsPreloaded { get; }

        void InvalidateCache();

        Task<IRichReportTemplateDataInfo> LoadTemplateAsync(CancellationToken cancellationToken = default);

        Task PreLoadAsync();
    }
}
