using ISoft.Utils.Data.AsyncData;
using System;
using System.Threading;

namespace ISoft.Wpf.RichReports.Helpers
{
    internal sealed class ReportTemplatesPreviewLoader : IDisposable
    {
        private static readonly object _syncLock = new();

        private static readonly Lazy<ReportTemplatesPreviewLoader> _lazyFallbackInstance =
            new(() => new(),
                LazyThreadSafetyMode.ExecutionAndPublication);

        private static ReportTemplatesPreviewLoader _currentInstance;

        /// <summary>
        /// Gets the current active user options manager.
        /// </summary>
        public static ReportTemplatesPreviewLoader Current
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
        private bool _disposed;

        public void Load(AsyncTask asyncTask)
        {
            EnsureNotDisposed();
            _backgroundWorker.ReplaceOrAddTask(asyncTask);
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _backgroundWorker.Dispose();
            GC.SuppressFinalize(this);
        }

        private void EnsureNotDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(nameof(ReportTemplatesPreviewCache));
        }
    }
}
