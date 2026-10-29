using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Abstractions.Templates
{
    /// <summary>
    /// Provider for default/built-in templates
    /// </summary>
    public interface IRichDefaultTemplateInfoProvider
    {
        Task<IRichReportTemplateDataInfo> GetReportTemplateAsync(Guid? templateId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get all default templates for a report
        /// </summary>
        Task<IEnumerable<IRichReportTemplateDataInfo>> GetReportTemplatesAsync(int? reportId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if default templates exist for this report
        /// </summary>
        Task<bool> HasTemplatesAsync(int? reportId);

        void InvalidateCache(int? reportId = null);

        Task PreLoadAllAsync(CancellationToken cancellationToken = default);

        Task PreLoadAsync(int? reportId, CancellationToken cancellationToken = default);
    }
}
