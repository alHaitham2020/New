using ISoft.RichReports.Core.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Client
{
    public interface IRichReportCalculatedFieldStorage
    {
        Task<bool> DeleteAsync(Guid? id);

        Task<bool> ExistsAsync(Guid? id);

        IRichReportCalculatedFieldInfo FindByName(string name);

        Task<IRichReportCalculatedFieldInfo> FindByNameAsync(string name);

        Task<IRichReportCalculatedFieldInfo> GetByKeyAsync(Guid? id);

        IEnumerable<IRichReportCalculatedFieldInfo> GetCalculatedFields();

        bool IsDuplicate(Guid? id,string name);

        bool IsDuplicateDisplayName(Guid? id, string name);

        Task<bool> IsDuplicateAsync(Guid? id, string name);

        Task<bool> RenameAsync(Guid? id, string newName);

        Guid? Save(IRichReportCalculatedFieldInfo fieldInfo);

        Task<IRichReportCalculatedFieldInfo> SaveAsync(IRichReportCalculatedFieldInfo fieldInfo);

        Task<bool> IsDuplicateDisplayNameAsync(Guid? id, string name);
    }
}
