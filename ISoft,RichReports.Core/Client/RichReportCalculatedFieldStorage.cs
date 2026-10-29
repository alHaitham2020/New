using ISoft.Printing.Shared.Data;
using ISoft.RichReports.Core.Data;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Client
{
    public class RichReportCalculatedFieldStorage : IRichReportCalculatedFieldStorage
    {
        private readonly IReportInfo _reportInfo;
        private readonly IRichReportCalculatedFieldInfoProvider _provider;

        public RichReportCalculatedFieldStorage(
            IReportInfo reportInfo,
            IRichReportCalculatedFieldInfoProvider provider)
        {
            _reportInfo = reportInfo;
            _provider = provider;
        }

        public IEnumerable<IRichReportCalculatedFieldInfo> GetCalculatedFields() => _provider.GetReportFields(_reportInfo.Id);

        public async Task<bool> DeleteAsync(Guid? id) => await _provider.DeleteAsync(id);

        public bool IsDuplicate(Guid? id, string name) => _provider.IsDuplicate(_reportInfo.Id, id, name);

        public async Task<bool> ExistsAsync(Guid? id) => await _provider.ExistsAsync(id);

        public IRichReportCalculatedFieldInfo FindByName(string name) => _provider.Find(_reportInfo.Id, name);

        public async Task<IRichReportCalculatedFieldInfo> GetByKeyAsync(Guid? id) => await _provider.GetByKeyAsync(id);

        public async Task<bool> IsDuplicateAsync(Guid? id, string name) => await _provider.IsDuplicateAsync(_reportInfo.Id, id, name);

        public async Task<bool> RenameAsync(Guid? id, string newName) => await _provider.RenameAsync(id, newName);

        public async Task<IRichReportCalculatedFieldInfo> FindByNameAsync(string name) => await _provider.FindAsync(_reportInfo.Id, name);

        public bool IsDuplicateDisplayName(Guid? id, string name) => _provider.IsDuplicateDisplayName(_reportInfo.Id, id, name);

        public async Task<bool> IsDuplicateDisplayNameAsync(Guid? id, string name) => await _provider.IsDuplicateDisplayNameAsync(_reportInfo.Id, id, name);

        public async Task<IRichReportCalculatedFieldInfo> SaveAsync(IRichReportCalculatedFieldInfo fieldInfo)
        {
            ArgumentGuardHelper.ArgumentNotNull(fieldInfo, nameof(fieldInfo));

            var addedKey = await _provider.SaveAsync(fieldInfo);
            if (addedKey.IsNullOrEmpty())
            {
                return null;
            }

            // return the newlly item with new key
            return await GetByKeyAsync(addedKey);
        }

        public Guid? Save(IRichReportCalculatedFieldInfo fieldInfo)
        {
            ArgumentGuardHelper.ArgumentNotNull(fieldInfo, nameof(fieldInfo));
            return _provider.Save(fieldInfo);
        }
    }
}
