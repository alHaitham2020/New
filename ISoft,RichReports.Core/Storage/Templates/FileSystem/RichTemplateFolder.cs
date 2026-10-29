using ISoft.Utils.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Templates.FileSystem
{
    /// <summary>
    /// Physical template folder implementation with async support
    /// </summary>
    public class RichTemplateFolder : IRichTemplateFolder
    {
        private readonly IRichTemplateStorageConfig _config;
        private readonly ConcurrentCacheDictionary<string, IRichTemplateFile> _files;
        private readonly SemaphoreSlim _loadLock;
        private volatile bool _isLoaded;
        private DateTime _lastLoadTime;

        public RichTemplateFolder(string reportId, string name, IRichTemplateStorageConfig config)
        {
            ReportId = reportId ?? throw new ArgumentNullException(nameof(reportId));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _files = new ConcurrentCacheDictionary<string, IRichTemplateFile>(StringComparer.OrdinalIgnoreCase);
            _loadLock = new SemaphoreSlim(1, 1);
        }

        #region Properties

        public string Name { get; }

        public string ReportId { get; }

        public string FullPath => Path.Combine(_config.GetTemplatesPath(), Name);

        public bool Exists => Directory.Exists(FullPath);

        public bool IsLoaded => _isLoaded;

        public DateTime LastLoadTime => _lastLoadTime;

        public int FileCount => _files.Count;

        #endregion

        #region Synchronous Access

        public IEnumerable<IRichTemplateFile> Files
        {
            get
            {
                EnsureLoaded();
                return _files.Values.ToList();
            }
        }

        public IRichTemplateFile GetFile(string fileName)
        {
            EnsureLoaded();
            return _files.TryGetValue(fileName, out var file) ? file : null;
        }

        public IRichTemplateFile AddFile(string fileName)
        {
            EnsureLoaded();

            return _files.GetOrAdd(fileName, key =>
            {
                var file = new RichTemplateFile(this, key, _config);
                return file;
            });
        }

        public IEnumerable<IRichTemplateFile> GetFiles()
        {
            if (!Exists) return Enumerable.Empty<IRichTemplateFile>();

            EnsureLoaded();
            return _files.Values.ToList();
        }

        #endregion

        #region Async Access

        public async Task<IRichTemplateFile> GetFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            await EnsureLoadedAsync(cancellationToken);
            return _files.TryGetValue(fileName, out var file) ? file : null;
        }

        public async Task<IRichTemplateFile> AddFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            await EnsureLoadedAsync(cancellationToken);

            return _files.GetOrAdd(fileName, key =>
            {
                var file = new RichTemplateFile(this, key, _config);
                return file;
            });
        }

        public async Task<IEnumerable<IRichTemplateFile>> GetFilesAsync(CancellationToken cancellationToken = default)
        {
            if (!Exists) return Enumerable.Empty<IRichTemplateFile>();

            await EnsureLoadedAsync(cancellationToken);
            return _files.Values.ToList();
        }

        public async Task<IEnumerable<IRichTemplateFile>> LoadAllTemplateFilesAsync(CancellationToken cancellationToken = default)
        {
            if (!Exists) return Enumerable.Empty<IRichTemplateFile>();

            var pattern = $"*{_config.FileExtension}";
            var loadedFiles = new List<IRichTemplateFile>();

            await Task.Run(async () =>
            {
                var filePaths = Directory.GetFiles(FullPath, pattern, SearchOption.TopDirectoryOnly);

                foreach (var filePath in filePaths)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var fileName = Path.GetFileNameWithoutExtension(filePath);
                    var file = _files.GetOrAdd(fileName, key =>
                    {
                        return new RichTemplateFile(this, key, _config);
                    });

                    await file.PreLoadAsync();

                    loadedFiles.Add(file);
                }
            }, cancellationToken);

            _lastLoadTime = DateTime.UtcNow;
            return loadedFiles;
        }

        #endregion

        #region Loading

        public void EnsureLoaded()
        {
            if (_isLoaded) return;

            try
            {
                _loadLock.Wait();

                if (_isLoaded) return; // Double-check after acquiring lock

                LoadFilesInternal();
                _isLoaded = true;
                _lastLoadTime = DateTime.UtcNow;
            }
            finally
            {
                _loadLock.Release();
            }
        }

        public async Task EnsureLoadedAsync(CancellationToken cancellationToken = default)
        {
            if (_isLoaded) return;

            await _loadLock.WaitAsync(cancellationToken);

            try
            {
                if (_isLoaded) return; // Double-check after acquiring lock

                await Task.Run(() => LoadFilesInternal(), cancellationToken);
                _isLoaded = true;
                _lastLoadTime = DateTime.UtcNow;
            }
            finally
            {
                _loadLock.Release();
            }
        }

        private void LoadFilesInternal()
        {
            if (!Exists) return;

            var pattern = $"*{_config.FileExtension}";
            var filePaths = Directory.GetFiles(FullPath, pattern, SearchOption.TopDirectoryOnly);

            foreach (var filePath in filePaths)
            {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                _files.TryAdd(fileName, new RichTemplateFile(this, fileName, _config));
            }
        }

        public async Task ReloadAsync(CancellationToken cancellationToken = default)
        {
            await _loadLock.WaitAsync(cancellationToken);

            try
            {
                _files.Clear();
                _isLoaded = false;
                _lastLoadTime = default;
            }
            finally
            {
                _loadLock.Release();
            }

            await EnsureLoadedAsync(cancellationToken);
        }

        #endregion

        #region File Operations

        public async Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            await EnsureLoadedAsync(cancellationToken);

            if (_files.TryRemove(fileName, out var file))
            {
                try
                {
                    if (file.Exists)
                    {
                        File.Delete(file.FullPath);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    // Re-add on failure
                    _files.TryAdd(fileName, file);
                    throw new IOException($"Failed to delete template file: {fileName}", ex);
                }
            }

            return false;
        }

        public async Task<bool> RenameFileAsync(string oldFileName, string newFileName, CancellationToken cancellationToken = default)
        {
            await EnsureLoadedAsync(cancellationToken);

            if (_files.TryRemove(oldFileName, out var file))
            {
                try
                {
                    var newFile = new RichTemplateFile(this, newFileName, _config);

                    if (file.Exists)
                    {
                        File.Move(file.FullPath, newFile.FullPath);
                    }

                    _files.TryAdd(newFileName, newFile);
                    return true;
                }
                catch (Exception ex)
                {
                    // Re-add old file on failure
                    _files.TryAdd(oldFileName, file);
                    throw new IOException($"Failed to rename template file: {oldFileName} -> {newFileName}", ex);
                }
            }

            return false;
        }

        public void EnsureExists()
        {
            try
            {
                if (!Exists)
                {
                    Directory.CreateDirectory(FullPath);
                }
            }
            catch
            {

            }
        }

        public async Task EnsureExistsAsync()
        {
            try
            {
                if (!Exists)
                {
                    await Task.Run(() => Directory.CreateDirectory(FullPath));
                }
            }
            catch
            {

            }
        }

        #endregion

        #region Utility

        public async Task<int> GetFileCountAsync(CancellationToken cancellationToken = default)
        {
            await EnsureLoadedAsync(cancellationToken);
            return _files.Count;
        }

        public bool HasFile(string fileName)
        {
            EnsureLoaded();
            return _files.ContainsKey(fileName);
        }

        public async Task<bool> HasFileAsync(string fileName, CancellationToken cancellationToken = default)
        {
            await EnsureLoadedAsync(cancellationToken);
            return _files.ContainsKey(fileName);
        }

        public void InvalidateCache()
        {
            _isLoaded = false;
            _files.Clear();
            _lastLoadTime = default;
        }

        public override string ToString() => $"{Name} [{FullPath}] ({_files.Count} files)";

        #endregion
    }


}
