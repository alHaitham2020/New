using ISoft.RichReports.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Client
{
    public interface IRichReportCalculatedFieldInfoProvider
    {
        Task<bool> DeleteAsync(Guid? id, CancellationToken ct = default);

        Task<bool> ExistsAsync(Guid? id, CancellationToken ct = default);

        Task<IRichReportCalculatedFieldInfo> FindAsync(int? reportId, string name);

        IRichReportCalculatedFieldInfo Find(int? reportId, string name);

        Task<IRichReportCalculatedFieldInfo> GetByKeyAsync(Guid? id);

        IEnumerable<IRichReportCalculatedFieldInfo> GetReportFields(int? reportId);

        bool IsDuplicate(int? reportId, Guid? id, string name);

        bool IsDuplicateDisplayName(int? reportId, Guid? id, string displayName);

        Task<bool> IsDuplicateAsync(int? reportId, Guid? id, string name, CancellationToken ct = default);

        Task<bool> IsDuplicateDisplayNameAsync(int? reportId, Guid? id, string displayName, CancellationToken ct = default);

        Task<bool> RenameAsync(Guid? id, string newName, CancellationToken ct = default);

        Task<Guid?> SaveAsync(IRichReportCalculatedFieldInfo fieldInfo, CancellationToken ct = default);

        Guid? Save(IRichReportCalculatedFieldInfo fieldInfo);
    }
}
