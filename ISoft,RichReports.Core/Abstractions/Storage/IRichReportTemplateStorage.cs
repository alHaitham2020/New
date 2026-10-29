using ISoft.RichReports.Core.Abstractions.Templates;
using ISoft.RichReports.Core.Models.Templates;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Abstractions.Storage
{
    public interface IRichReportTemplateStorage
    {
        IRichReportTemplate CreateNew();

        Task<IRichReportTemplate> CreateNewAsync(byte[] content);

        Task<IRichReportTemplate> CreateFromAsync(Guid? templateKey);

        Task<IRichReportTemplate> CreateNewAsync();

        IRichReportTemplate Load(Guid? key);

        Task<IRichReportTemplate> LoadAsync(Guid? templateKey);

        Task<RichReportTemplateSaveResult> SaveAsAsync(IRichReportTemplate template, string newName, CancellationToken ct = default);

        Task<RichReportTemplateSaveResult> SaveAsync(IRichReportTemplate template, CancellationToken ct = default);

        Task<bool> RenameAsync(Guid? templateKey, string templateName);

        Task<bool> DeleteAsync(Guid? templateKey);

        bool Export(IRichReportTemplate template);

        IRichReportTemplate Import();

        Task<IAsyncExecutionResult<IRichReportTemplate>> ImportAsync();

        bool Exists(Guid? templateKey);

        Task<bool> ExistsAsync(Guid? templateKey);

        bool IsDuplicate(string name);

        string GetNewName();

        Task<string> GetNewNameAsync();

        Task<bool> IsDuplicateAsync(string name);
    }
}
