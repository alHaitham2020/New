using ISoft.RichReports.Core.Abstractions.Storage.Templates;
using ISoft.RichReports.Core.Abstractions.Templates;
using ISoft.RichReports.Core.Templates.Serialization;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Storage.Templates.FileSystem
{
    /// <summary>
    /// Extended template file with pre-loading support
    /// </summary>
    public class RichTemplateFile : IRichTemplateFile
    {
        private readonly IRichTemplateStorageConfig _config;
        private readonly SemaphoreSlim _loadLock;
        private IRichReportTemplateDataInfo _cachedTemplate;
        private bool _isPreloaded;

        public RichTemplateFile(IRichTemplateFolder folder, string name, IRichTemplateStorageConfig config)
        {
            Folder = folder ?? throw new ArgumentNullException(nameof(folder));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _loadLock = new SemaphoreSlim(1, 1);
        }

        public string Name { get; }

        public IRichTemplateFolder Folder { get; }

        public string FullPath => Path.Combine(Folder.FullPath, $"{Name}{_config.FileExtension}");

        public bool Exists => File.Exists(FullPath);

        public bool IsPreloaded => _isPreloaded;

        public async Task<IRichReportTemplateDataInfo> LoadTemplateAsync(CancellationToken cancellationToken = default)
        {
            if (_cachedTemplate != null)
                return _cachedTemplate;

            await _loadLock.WaitAsync(cancellationToken);

            try
            {
                if (_cachedTemplate != null) // Double-check
                    return _cachedTemplate;

                if (!Exists)
                    throw new FileNotFoundException($"Template not found: {FullPath}");

                _cachedTemplate = await DeserializeTemplateAsync();
                return _cachedTemplate;
            }
            finally
            {
                _loadLock.Release();
            }
        }

        public async Task PreLoadAsync()
        {
            if (_isPreloaded) return;

            try
            {
                await LoadTemplateAsync();
                _isPreloaded = true;
            }
            catch
            {
                // Pre-load failure is non-critical
                _isPreloaded = false;
            }
        }

        private async Task<IRichReportTemplateDataInfo> DeserializeTemplateAsync()
        {
            var template =  await RichReportTemplateSerializationHelper.DeserializeFromFileAsync(FullPath);
            if (template == null) return null;

            return new RichReportTemplateDataInfo
            {
                Id = template.Id,
                Name = template.Name,
                Thumbnail = template.Thumbnail,
                Content = template.Content,
                Version = template.Version,
                IsShared = true,
                ModifiedDate = DateTime.Now,
                OwnerId = Guid.Empty,
                OwnerName = string.Empty
            };
        }

        public void InvalidateCache()
        {
            _cachedTemplate = null;
            _isPreloaded = false;
        }

        public override string ToString() => FullPath;
    }
}
