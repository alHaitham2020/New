using ISoft.Printing.Core.Xpf.BandDesigner;
using ISoft.Utils.Caching;
using ISoft.Utils.Data.AsyncData;
using ISoft.Utils.Helpers;
using System;
using System.Threading;
using System.Windows.Media.Imaging;

namespace ISoft.Wpf.RichReports.Helpers
{
    internal sealed class ReportTemplatesPreviewCache : IDisposable
    {
        private static readonly object _syncLock = new();

        private static readonly Lazy<ReportTemplatesPreviewCache> _lazyFallbackInstance =
            new(() => new(),
                LazyThreadSafetyMode.ExecutionAndPublication);

        private static ReportTemplatesPreviewCache _currentInstance;

        /// <summary>
        /// Gets the current active user options manager.
        /// </summary>
        public static ReportTemplatesPreviewCache Current
        {
            get
            {
                lock (_syncLock)
                {
                    return _currentInstance ??= _lazyFallbackInstance.Value;
                }
            }
        }

        private readonly AsyncParallelBackgroundWorker _backgroundWorker = new();
        private readonly LimitedCache<Guid, BitmapImage> _cache = new(50);
        private bool _disposed;

        public void Load(AsyncTask asyncTask)
        {
            EnsureNotDisposed();
            _backgroundWorker.ReplaceOrAddTask(asyncTask);
        }

        public bool TryGet(Guid? key, out BitmapImage result)
        {
            EnsureNotDisposed();

            if (key.IsNullOrEmpty())
            {
                result = null;
                return false;
            }

            return _cache.TryGetValue(key.Value, out result);
        }

        public void AddToCache(Guid? key, BitmapImage template)
        {
            EnsureNotDisposed();

            if (key.IsNullOrEmpty()) return;
            if (template == null) return;

            _cache.AddOrUpdate(key.Value, template);
        }

        public void RemoveFromCache(Guid? key)
        {
            EnsureNotDisposed();

            if (key.IsNullOrEmpty()) return;

            _cache.Remove(key.Value);
        }

        public void ClearCache()
        {
            EnsureNotDisposed();
            _cache.Clear();
        }

        public void ClearExpiredValues()
        {
            EnsureNotDisposed();
            _cache.ClearExpiredValues();
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _backgroundWorker.Dispose();
            _cache.Dispose();
            GC.SuppressFinalize(this);
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ReportTemplatesPreviewCache));
        }
    }
}
