using System.Collections.ObjectModel;

namespace ISoft.Wpf.RichReports.Models
{
    /// <summary>
    /// Thread-safe template items collection with async loading support
    /// </summary>
    public abstract class RichReportTemplateItemsSourceBase : AsyncCollectionSourceLoaderBase<IXpfRichReportTemplatePreviewItem>, IXpfRichReportTemplateItemsSource
    {
        private readonly SemaphoreSlim _lock = new(1, 1);

        IAsyncCollectionDataSourceProvider<IXpfRichReportTemplatePreviewItem> IXpfRichReportTemplateItemsSource.Source => Source;

        protected override bool AllowAutoLoading => false;

        protected RichReportTemplateItemsSourceBase()
        {
           
        }

        #region Actions

        public async Task<IXpfRichReportTemplatePreviewItem> FindFromStoreAsync(Guid? key)
        {
            if (key.IsNullOrEmpty()) return null;

            var template = await GetByKeyAsync(key);

            if (template == null) return null;

            return CreateItem(template);
        }

        // Find item in local collection
        public IXpfRichReportTemplatePreviewItem FindLocal(Guid? key)
        {
            if (key.IsNullOrEmpty() || !Source.HasData)
                return null;

            // Use FirstOrDefault with null check for better performance
            return Source.DataSource.FirstOrDefault(x => x.Id == key);
        }

        public async Task OnItemChangedAsync(DataChangeMessageType changeType, Guid? targetId)
        {
            switch (changeType)
            {
                case DataChangeMessageType.Insert: await OnItemAddedAsync(targetId); break;
                case DataChangeMessageType.Update: await OnItemUpdatedAsync(targetId); break;
                case DataChangeMessageType.Delete: await OnItemDeletedAsync(targetId); break;
                default: throw new NotImplementedException(nameof(changeType));
            }
        }

        // Thread-safe method to add item to collection
        public async Task OnItemAddedAsync(Guid? key)
        {
            if (key.IsNullOrEmpty()) return;

            await _lock.ExecuteAsync(async () =>
            {
                var storeItem = await FindFromStoreAsync(key);
                if (storeItem != null && FindLocal(key) == null)
                {
                    Source.DataSource.Add(storeItem);
                }
            });
        }

        // Thread-safe method to remove item from collection
        public async Task OnItemDeletedAsync(Guid? key)
        {
            if (key.IsNullOrEmpty()) return;

            await _lock.ExecuteAsync(async () =>
            {
                var localItem = FindLocal(key);
                if (localItem != null)
                {
                    Source.DataSource.Remove(localItem);
                }
            });
        }

        public async Task OnItemUpdatedAsync(Guid? key)
        {
            if (key.IsNullOrEmpty()) return;

            var localItem = FindLocal(key);
            var storeItem = await FindFromStoreAsync(key);

            var index = Source.DataSource.IndexOf(localItem);
            if (index >= 0 && localItem != null)
            {
                Source.DataSource.RemoveAt(index);
            }

            // Update existing item
            if (index >= 0 && storeItem != null)
            {
                storeItem.RefreshPreview();
                Source.DataSource.Insert(index, storeItem);
            }
        }

        protected virtual XpfRichReportTemplatePreviewItem CreateItem(IRichReportTemplateDataInfo template)
        {
            return ViewModelSource.Create(() => new XpfRichReportTemplatePreviewItem()
            {
                Id = template.Id,
                Name = template.Name,
                ThumbinalData = template.Thumbnail,
                Content = template.Content,
                IsShared = template.IsShared,
                ModifiedDate = template.ModifiedDate,
            });
        }
        #endregion

        #region Ovverides

        protected abstract Task<List<IXpfRichReportTemplatePreviewItem>> GetTemplatePreviewItemsAsync(CancellationToken cancellationToken = default);
        
        protected abstract Task<IRichReportTemplateDataInfo> GetByKeyAsync(Guid? templateId, CancellationToken cancellationToken = default);

        // Async data loading with cancellation support
        public sealed override async Task<ObservableCollection<IXpfRichReportTemplatePreviewItem>> GetDataSourceAsync(bool forceLoad, CancellationToken token)
        {
            try
            {
                var items = await GetTemplatePreviewItemsAsync(token);
                return new ObservableCollection<IXpfRichReportTemplatePreviewItem>(items.OrderByDescending(x => x.ModifiedDate));
            }
            catch (OperationCanceledException)
            {
                // Return empty collection if operation was cancelled
                return new();
            }
        }
        #endregion
    }
}
