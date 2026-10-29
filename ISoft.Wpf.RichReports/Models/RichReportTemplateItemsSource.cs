namespace ISoft.Wpf.RichReports.Models
{
    /// <summary>
    /// Thread-safe template items collection with async loading support
    /// </summary>
    public class RichReportTemplateItemsSource : RichReportTemplateItemsSourceBase
    {
        private readonly IRichReportTemplateSourceFilterOptions _filterOptions;
        private readonly IRichReportTemplateInfoProvider _itemsProviderService;

        protected RichReportTemplateItemsSource(
            IRichReportTemplateInfoProvider itemsProviderService,
            IRichReportTemplateSourceFilterOptions filterOptions)
        {
            _itemsProviderService = itemsProviderService ?? throw new ArgumentNullException(nameof(itemsProviderService));
            _filterOptions = filterOptions;
        }

        #region Actions

        public IXpfRichReportTemplatePreviewItem FindOrGetFirstFromStore(Guid? key)
        {
            var templates = GetTemplateItems();

            return templates.Where(x => x.Id == key).FirstOrDefault() ?? templates.FirstOrDefault();
        }

        // Retrieve item from data store
        public IXpfRichReportTemplatePreviewItem FindFromStore(Guid? key)
        {
            if (key.IsNullOrEmpty()) return null;

            var template = _itemsProviderService.GetByKey(key);

            if (template == null) return null;

            return CreateItem(template);
        }

        // Get reports from provider
        private List<IXpfRichReportTemplatePreviewItem> GetTemplateItems()
        {
            var templatesSource = _itemsProviderService.GetReportTemplates(_filterOptions.ReportInfo.Id, _filterOptions.SourceType);

            if (templatesSource == null) return [];

            return [.. templatesSource.Select(CreateItem)];
        }
        #endregion

        #region Ovverides
        protected override async Task<List<IXpfRichReportTemplatePreviewItem>> GetTemplatePreviewItemsAsync(CancellationToken token = default)
        {
            var templatesSource = await _itemsProviderService.GetReportTemplatesAsync(_filterOptions.ReportInfo.Id, _filterOptions.SourceType);

            if (templatesSource == null) return [];

            return [.. templatesSource.Select(CreateItem)];
        }

        protected override async Task<IRichReportTemplateDataInfo> GetByKeyAsync(Guid? templateId, CancellationToken cancellationToken = default)
        {
            return await _itemsProviderService.GetByKeyAsync(templateId);
        }
        #endregion

        #region Factory
        // Factory methods for creating source instances

        internal static RichReportTemplateItemsSource Create(
            IRichReportTemplateInfoProvider itemsProvider,
            IReportInfo reportInfo,
            RichReportTemplateSourceType sourceType)
        {
            return Create(itemsProvider, RichReportTemplateSourceFilterOptions.Create(reportInfo, sourceType));
        }

        internal static RichReportTemplateItemsSource Create(
            IRichReportTemplateInfoProvider itemsProvider,
            IRichReportTemplateSourceFilterOptions filterOptions)
        {
            return ViewModelSource.Create(() => new RichReportTemplateItemsSource(itemsProvider, filterOptions));
        }
        #endregion
    }
}
