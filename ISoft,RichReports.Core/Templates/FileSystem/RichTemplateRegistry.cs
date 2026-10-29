using DevExpress.Mvvm.Native;
using ISoft.Utils.Caching;
using ISoft.Utils.Extentions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Templates.FileSystem
{
    /// <summary>
    /// Thread-safe template registry with async support
    /// </summary>
    public class RichTemplateRegistry : IRichTemplateRegistry
    {
        private readonly IRichTemplateStorageConfig _config;
        private readonly ConcurrentCacheDictionary<IRichTemplateFolderKey, RichTemplateFolder> _folders;
        private readonly SemaphoreSlim _globalLock;

        public IEnumerable<IRichTemplateFolder> AllFolders => _folders.Values;

        public RichTemplateRegistry(IRichTemplateStorageConfig config = null)
        {
            _config = config ?? new RichTemplateStorageConfig();
            _folders = new ();
            _globalLock = new SemaphoreSlim(1, 1);
        }

        #region Synchronous Registration

        public IRichTemplateRegistry RegisterFolder(IRichTemplateFolderKey key)
        {
            _folders.GetOrAdd(key, k => new RichTemplateFolder(k.ReportId, k.FolderName, _config));
            return this;
        }

        public IRichTemplateRegistry RegisterRange(params IRichTemplateFolderKey[] folders)
        {
            foreach (var key in folders)
            {
                RegisterFolder(key);
            }
            return this;
        }

        public IRichTemplateRegistry AddFiles(IRichTemplateFolderKey key, params string[] fileNames)
        {
            var folder = GetOrCreateFolder(key);

            foreach (var fileName in fileNames)
            {
                folder.AddFile(fileName);
            }

            return this;
        }

        #endregion

        #region Async Registration

        public async Task<IRichTemplateRegistry> RegisterFolderAsync(IRichTemplateFolderKey key, CancellationToken cancellationToken = default)
        {
            await _globalLock.WaitAsync(cancellationToken);
            try
            {
                return RegisterFolder(key);
            }
            finally
            {
                _globalLock.Release();
            }
        }

        public async Task<IRichTemplateRegistry> AddFilesAsync(IRichTemplateFolderKey key,string[] fileNames, CancellationToken cancellationToken = default)
        {
            var folder = await GetOrCreateFolderAsync(key, cancellationToken);

            foreach (var fileName in fileNames)
            {
                await folder.AddFileAsync(fileName, cancellationToken);
            }

            return this;
        }

        #endregion

        #region Retrieval

        public IRichTemplateFolder GetFolder(string reportId)
        {
            return _folders
                    .Where(kvp => kvp.Key.ReportId == reportId)
                    .Select(kvp => kvp.Value)
                    .FirstOrDefault();
        }

        public IRichTemplateFolder GetFolder(IRichTemplateFolderKey key)
        {
            return _folders.TryGetValue(key, out var folder) ? folder : null;
        }

        public async Task<IRichTemplateFolder> GetFolderAsync(string reportId, CancellationToken cancellationToken = default)
        {
            var folder = GetFolder(reportId);

            if (folder == null) return null;

            await folder.EnsureLoadedAsync(cancellationToken);

            return folder;
        }

        public IEnumerable<IRichTemplateFolder> GetFolders(string reportId)
        {
            return _folders
                    .Where(kvp => kvp.Key.ReportId == reportId)
                    .Select(kvp => kvp.Value)
                    .ToList();
        }

        public async Task<IEnumerable<IRichTemplateFolder>> GetFoldersAsync(string reportId, CancellationToken cancellationToken = default)
        {
            var folders = GetFolders(reportId);

            // Ensure all folders are loaded
            await folders
                  .Select(f => f.EnsureLoadedAsync(cancellationToken))
                  .Where(t => t != null)
                  .WhenAllAsync();

            return folders;
        }

        public IEnumerable<IRichTemplateFile> GetFiles(string reportId)
        {
            return GetFolders(reportId).SelectMany(f => f.Files);
        }

        public async Task<IEnumerable<IRichTemplateFile>> GetFilesAsync(string reportId, CancellationToken cancellationToken = default)
        {
            var folders = await GetFoldersAsync(reportId, cancellationToken);
            var allFiles = new List<IRichTemplateFile>();

            foreach (var folder in folders)
            {
                var files = await folder.GetFilesAsync(cancellationToken);
                allFiles.AddRange(files);
            }

            return allFiles;
        }

        public IRichTemplateFile GetFile(string reportId, string fileName)
        {
            return GetFolders(reportId)
                    .Select(f => f.GetFile(fileName))
                    .FirstOrDefault(f => f != null);
        }

        public async Task<IRichTemplateFile> GetFileAsync(string reportId, string fileName, CancellationToken cancellationToken = default)
        {
            var folders = await GetFoldersAsync(reportId, cancellationToken);

            foreach (var folder in folders)
            {
                var file = await folder.GetFileAsync(fileName, cancellationToken);
                if (file != null) return file;
            }

            return null;
        }

        public bool HasTemplates(string reportId)
        {
            return _folders.Keys.Any(k => k.ReportId == reportId);
        }

        public IEnumerable<string> GetReportIds()
        {
            return _folders.Keys.Select(k => k.ReportId).Distinct().ToList();
        }

        #endregion

        #region Load Operations

        /// <summary>
        /// Load all templates for a report (with pre-loading)
        /// </summary>
        public async Task PreLoadAllTemplatesAsync(string reportId, CancellationToken cancellationToken = default)
        {
            var folders = GetFolders(reportId);

            var loadTasks = folders.Select(async folder =>
            {
                await folder.EnsureLoadedAsync(cancellationToken);
                await folder.LoadAllTemplateFilesAsync(cancellationToken);
            });

            await Task.WhenAll(loadTasks);
        }

        /// <summary>
        /// Load all templates for all reports
        /// </summary>
        public async Task PreLoadAllTemplatesAsync(CancellationToken cancellationToken = default)
        {
            var folders = _folders.Values;

            var loadTasks = folders.Select(async folder =>
            {
                await folder.EnsureLoadedAsync(cancellationToken);
                await folder.LoadAllTemplateFilesAsync(cancellationToken);
            });

            await Task.WhenAll(loadTasks);
        }

        /// <summary>
        /// Reload all templates (clears cache)
        /// </summary>
        public async Task ReloadAllAsync(CancellationToken cancellationToken = default)
        {
            var folders = _folders.Values;

            var reloadTasks = folders.Select(f => f.ReloadAsync(cancellationToken));
            await Task.WhenAll(reloadTasks);
        }

        /// <summary>
        /// Invalidate all caches
        /// </summary>
        public void InvalidateAllCaches()
        {
            foreach (var folder in _folders.Values)
            {
                folder.InvalidateCache();
            }
        }

        #endregion

        #region File Operations

        public async Task<bool> DeleteTemplateFileAsync(string reportId, string fileName, CancellationToken cancellationToken = default)
        {
            var folders = GetFolders(reportId);

            foreach (var folder in folders)
            {
                if (await folder.HasFileAsync(fileName, cancellationToken))
                {
                    return await folder.DeleteFileAsync(fileName, cancellationToken);
                }
            }

            return false;
        }

        #endregion

        #region Helpers

        private RichTemplateFolder GetOrCreateFolder(IRichTemplateFolderKey key)
        {
            RegisterFolder(key);
            return _folders[key];
        }

        private async Task<RichTemplateFolder> GetOrCreateFolderAsync(IRichTemplateFolderKey key, CancellationToken cancellationToken)
        {
            await RegisterFolderAsync(key, cancellationToken);
            return _folders[key];
        }

        public void EnsureAllFoldersExist()
        {
            foreach (var folder in _folders.Values)
            {
                folder.EnsureExists();
            }
        }

        public async Task EnsureAllFoldersExistAsync(CancellationToken cancellationToken = default)
        {
            var tasks = _folders.Values.Select(f => f.EnsureExistsAsync());
            await Task.WhenAll(tasks);
        }

        public async Task<int> GetTotalFileCountAsync(CancellationToken cancellationToken = default)
        {
            var tasks = _folders.Values.Select(f => f.GetFileCountAsync(cancellationToken));
            var counts = await Task.WhenAll(tasks);
            return counts.Sum();
        }

        #endregion

        #region Disposable

        public void Dispose()
        {
            _globalLock?.Dispose();

            //foreach (var folder in _folders.Values)
            //{
            //    // Dispose folder resources if needed
            //}
        }

        #endregion
    }
}
