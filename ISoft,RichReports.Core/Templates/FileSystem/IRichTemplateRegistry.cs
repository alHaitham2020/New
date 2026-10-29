using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Templates.FileSystem
{
    public interface IRichTemplateRegistry
    {
        IEnumerable<IRichTemplateFolder> AllFolders { get; }

        IRichTemplateRegistry AddFiles(IRichTemplateFolderKey key, params string[] fileNames);

        Task<IRichTemplateRegistry> AddFilesAsync(IRichTemplateFolderKey key, string[] fileNames, CancellationToken cancellationToken = default);

        Task<bool> DeleteTemplateFileAsync(string reportId, string fileName, CancellationToken cancellationToken = default);

        void Dispose();

        void EnsureAllFoldersExist();

        Task EnsureAllFoldersExistAsync(CancellationToken cancellationToken = default);

        IRichTemplateFile GetFile(string reportId, string fileName);

        Task<IRichTemplateFile> GetFileAsync(string reportId, string fileName, CancellationToken cancellationToken = default);

        IEnumerable<IRichTemplateFile> GetFiles(string reportId);

        Task<IEnumerable<IRichTemplateFile>> GetFilesAsync(string reportId, CancellationToken cancellationToken = default);

        IRichTemplateFolder GetFolder(string reportId);

        IRichTemplateFolder GetFolder(IRichTemplateFolderKey key);

        Task<IRichTemplateFolder> GetFolderAsync(string reportId, CancellationToken cancellationToken = default);

        IEnumerable<IRichTemplateFolder> GetFolders(string reportId);

        Task<IEnumerable<IRichTemplateFolder>> GetFoldersAsync(string reportId, CancellationToken cancellationToken = default);

        IEnumerable<string> GetReportIds();

        Task<int> GetTotalFileCountAsync(CancellationToken cancellationToken = default);

        bool HasTemplates(string reportId);

        void InvalidateAllCaches();

        Task PreLoadAllTemplatesAsync(CancellationToken cancellationToken = default);

        Task PreLoadAllTemplatesAsync(string reportId, CancellationToken cancellationToken = default);

        IRichTemplateRegistry RegisterFolder(IRichTemplateFolderKey key);

        Task<IRichTemplateRegistry> RegisterFolderAsync(IRichTemplateFolderKey key, CancellationToken cancellationToken = default);

        IRichTemplateRegistry RegisterRange(params IRichTemplateFolderKey[] folders);

        Task ReloadAllAsync(CancellationToken cancellationToken = default);
    }

}
