using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Abstractions.Templates
{
    public interface IRichReportTemplateService
    {
        int CountByType(int? reportId);

        Task<int> CountByTypeAsync(int? reportId, CancellationToken ct = default);

        Guid? GetActiveTemplateKey(int? reportTypeId);

        Task<Guid?> GetActiveTemplateKeyAsync(int? reportTypeId, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid? templateKey);

        bool IsDuplicate(int? reportTypeId, string templateName);

        Task<bool> IsDuplicateAsync(int? reportTypeId, string templateName, CancellationToken ct = default);

        IRichReportTemplateData GetTemplateByKey(Guid? templateId);

        Task<IRichReportTemplateData> GetTemplateByKeyAsync(Guid? templateId);

        Task<DataValidationResult<Guid?>> RenameTemplateAsync(Guid? templateKey, string newName, CancellationToken ct = default);

        Task<DataValidationResult<Guid?>> SaveAsTemplateAsync(int? reportTypeId, IRichReportTemplateData templateData, CancellationToken ct = default);

        Task<DataValidationResult<Guid?>> SaveTemplateAsync(IRichReportTemplateData templateData, CancellationToken ct = default);

        Task<DataValidationResult<Guid?>> DeleteTemplateAsync(Guid? templateKey, CancellationToken ct = default);

        Task<DataValidationResult<Guid?>> UpdateIsSharedAsync(Guid? templateKey, bool isShared, CancellationToken ct = default);

        void SetActiveTemplateKey(int? reportTypeId, Guid? templateId);

        Task SetActiveTemplateKeyAsync(int? reportTypeId, Guid? templateId, CancellationToken ct = default);

    }
}
