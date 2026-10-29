using ISoft.RichReports.Core.Templates;
using ISoft.RichReports.Core.Templates.FileSystem;
using ISoft.Utils.Caching;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Client
{
    /// <summary>
    /// Optimized default template provider with caching and parallel loading
    /// </summary>
    public class RichRegistryDefaultTemplateProvider : IRichDefaultTemplateInfoProvider
    {
        private readonly IRichTemplateRegistry _registry;
        private readonly SemaphoreSlim _cacheLock;
        private readonly ConcurrentCacheDictionary<int, List<IRichReportTemplateDataInfo>> _cachedTemplates;
        private DateTime _lastCacheRefresh;

        public RichRegistryDefaultTemplateProvider(IRichTemplateRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _cacheLock = new SemaphoreSlim(1, 1);
            _cachedTemplates = new();
        }

        #region IRichDefaultTemplateInfoProvider
        /// <summary>
        /// Get all default templates for a report with caching
        /// </summary>
        public async Task<IEnumerable<IRichReportTemplateDataInfo>> GetReportTemplatesAsync(int? reportId, CancellationToken cancellationToken = default)
        {
            if (reportId == null)
                throw new ArgumentNullException(nameof(reportId));

            // Try cache first
            if (TryGetFromCache(reportId.Value, out var cachedTemplates))
                return cachedTemplates;

            await _cacheLock.WaitAsync(cancellationToken);
            try
            {
                // Double-check cache
                if (TryGetFromCache(reportId.Value, out cachedTemplates))
                    return cachedTemplates;

                var templates = await LoadTemplatesForReportAsync(reportId.Value,cancellationToken);

                // Update cache
                _cachedTemplates[reportId.Value] = templates;
                _lastCacheRefresh = DateTime.UtcNow;

                return templates;
            }
            finally
            {
                _cacheLock.Release();
            }
        }

        /// <summary>
        /// Get a specific template by ID (with efficient search)
        /// </summary>
        public async Task<IRichReportTemplateDataInfo> GetReportTemplateAsync(Guid? templateId, CancellationToken cancellationToken = default)
        {
            if (templateId.IsNullOrEmpty())
                return null;

            // Search in cache first
            foreach (var cachedList in _cachedTemplates.Values)
            {
                var found = cachedList.FirstOrDefault(t => t.Id == templateId);
                if (found != null) return found;
            }

            // Search in all folders (with early exit)
            foreach (var folder in _registry.AllFolders)
            {
                await folder.EnsureLoadedAsync(cancellationToken);

                foreach (var file in folder.Files)
                {
                    if (!file.IsPreloaded) continue;

                    var template = await file.LoadTemplateAsync(cancellationToken);
                    if (template != null && template.Id == templateId)
                    {
                        return template;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get a template by name (optimized with cache)
        /// </summary>
        public async Task<IRichReportTemplateDataInfo> GetReportTemplateByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            // Search in cache
            foreach (var cachedList in _cachedTemplates.Values)
            {
                var found = cachedList.FirstOrDefault(t =>
                    t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (found != null) return found;
            }

            // Search in folders
            foreach (var folder in _registry.AllFolders)
            {
                await folder.EnsureLoadedAsync(cancellationToken);

                var file = folder.GetFile(name);
                if (file != null)
                {
                    return await file.LoadTemplateAsync(cancellationToken);
                }
            }

            return null;
        }

        /// <summary>
        /// Check if templates exist for a report
        /// </summary>
        public async Task<bool> HasTemplatesAsync(int? reportId)
        {
            if (reportId == null) return false;

            // Check cache first
            if (_cachedTemplates.ContainsKey(reportId.Value) &&
                _cachedTemplates[reportId.Value].Count > 0)
                return true;

            return _registry.HasTemplates(reportId.ToString());
        }

        /// <summary>
        /// Pre-load all templates for a report (background initialization)
        /// </summary>
        public async Task PreLoadAsync(int? reportId, CancellationToken cancellationToken = default)
        {
            if (reportId == null) return;

            await _registry.PreLoadAllTemplatesAsync(reportId.ToString(), cancellationToken);

            // Populate cache
            await GetReportTemplatesAsync(reportId, cancellationToken);
        }

        /// <summary>
        /// Pre-load all templates (background initialization)
        /// </summary>
        public async Task PreLoadAllAsync(CancellationToken cancellationToken = default)
        {
            await _registry.PreLoadAllTemplatesAsync(cancellationToken);

            // Populate cache for all reports
            var reportIds = _registry.GetReportIds();
            foreach (var reportId in reportIds)
            {
                if (int.TryParse(reportId, out var id))
                {
                    await LoadTemplatesForReportAsync(id, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Invalidate cache to force reload
        /// </summary>
        public void InvalidateCache(int? reportId = null)
        {
            if (reportId.HasValue)
            {
                _cachedTemplates.TryRemove(reportId.Value,out _);
            }
            else
            {
                _cachedTemplates.Clear();
                _lastCacheRefresh = default;
            }
        }

        #endregion

        #region Private Helpers

        private bool TryGetFromCache(int reportId, out List<IRichReportTemplateDataInfo> templates)
        {
            if (_cachedTemplates.TryGetValue(reportId, out templates) &&
                templates.Count > 0)
            {
                return true;
            }

            templates = null;
            return false;
        }

        private async Task<List<IRichReportTemplateDataInfo>> LoadTemplatesForReportAsync(int reportId, CancellationToken cancellationToken = default)
        {
            var templates = new List<IRichReportTemplateDataInfo>();
            var reportIdStr = reportId.ToString();

            var folder = _registry.GetFolder(reportIdStr);
            if (folder == null) return templates;

            // Load templates in parallel for better performance
            var files = await folder.GetFilesAsync(cancellationToken);
            var loadTasks = files.Select(async file =>
            {
                try
                {
                    return await file.LoadTemplateAsync(cancellationToken);
                }
                catch
                {
                    return null;
                }
            });

            var results = await Task.WhenAll(loadTasks);
            templates.AddRange(results.Where(t => t != null));

            return templates;
        }

        #endregion
    }
}
