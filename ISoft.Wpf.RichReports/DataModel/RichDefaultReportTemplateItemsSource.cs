using DevExpress.Mvvm.POCO;
using ISoft.Printing.Shared.Data;
using ISoft.RichReports.Core.Client;
using ISoft.RichReports.Core.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.Wpf.RichReports.DataModel
{
    public class RichDefaultReportTemplateItemsSource : RichReportTemplateItemsSourceBase
    {
        private readonly IReportInfo _reportInfo;
        private readonly IRichDefaultTemplateInfoProvider _itemsProviderService;

        protected RichDefaultReportTemplateItemsSource(
            IRichDefaultTemplateInfoProvider itemsProviderService,
            IReportInfo reportInfo)
        {
            _itemsProviderService = itemsProviderService ?? throw new ArgumentNullException(nameof(itemsProviderService));
            _reportInfo = reportInfo;
        }

        #region Ovverides
        protected override async Task<List<IXpfRichReportTemplatePreviewItem>> GetTemplatePreviewItemsAsync(CancellationToken token = default)
        {
            var templatesSource = await _itemsProviderService.GetReportTemplatesAsync(_reportInfo.Id, token);

            if (templatesSource == null) return [];

            return [.. templatesSource.Select(CreateItem)];
        }

        protected override async Task<IRichReportTemplateDataInfo> GetByKeyAsync(Guid? templateId, CancellationToken cancellationToken = default)
        {
            return await _itemsProviderService.GetReportTemplateAsync(templateId, cancellationToken);
        }
        #endregion

        #region Factory
        /// <summary>
        /// Factory method for creating source instances
        /// </summary>
        /// <param name="itemsProvider"></param>
        /// <param name="reportInfo"></param>
        /// <returns></returns>
        internal static RichDefaultReportTemplateItemsSource Create(
            IRichDefaultTemplateInfoProvider itemsProvider,
            IReportInfo reportInfo)
        {
            return ViewModelSource.Create(() => new RichDefaultReportTemplateItemsSource(itemsProvider, reportInfo));
        }
        #endregion
    }

}
