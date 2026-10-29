using ISoft.Printing.Shared.Data;
using ISoft.RichReports.Core.Templates;
using ISoft.Utils.Data.AsyncData;
using ISoft.Utils.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Client
{
    public class RichReportTemplateStorage : IRichReportTemplateStorage
    {
        private readonly IReportInfo _reportInfo;
        private readonly IRichReportTemplateService _service;

        public RichReportTemplateStorage(IReportInfo reportInfo, IRichReportTemplateService service)
        {
            _reportInfo = reportInfo;
            _service = service;
        }

        #region IRichReportTemplateStorage
        public string GetNewName()
           => _service.GetNewName(_reportInfo.Id, _reportInfo.Title);

        public async Task<string> GetNewNameAsync()
            => await _service.GetNewNameAsync(_reportInfo.Id, _reportInfo.Title);

        public IRichReportTemplate CreateNew()
        {
            var name = GetNewName();

            return new RichReportTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
            };
        }

        public async Task<IRichReportTemplate> CreateNewAsync()
        {
            var name = await GetNewNameAsync();

            return new RichReportTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
            };
        }

        public async Task<IRichReportTemplate> CreateNewAsync(byte[] content)
        {
            var name = await GetNewNameAsync();

            return new RichReportTemplate
            {
                Id = Guid.NewGuid(),
                Name = name,
                Content = content
            };
        }

        public async Task<IRichReportTemplate> CreateFromAsync(Guid? key)
        {
            var sourceTemplate = await LoadAsync(key);

            if (sourceTemplate == null)
                return null;

            var name = await GetNewNameAsync();

            return sourceTemplate.CreateCopy(Guid.NewGuid(), name);
        }

        public async Task<IRichReportTemplate> LoadAsync(Guid? key)
        {
            if (key.IsNullOrEmpty())
                return null;

            var templateData = await _service.GetTemplateByKeyAsync(key);
            if (templateData == null) return null;

            return RichReportTemplate.CreateFromData(templateData);
        }

        public IRichReportTemplate Load(Guid? key)
        {
            if (key.IsNullOrEmpty())
                return null;

            if (key.IsNullOrEmpty())
                return null;

            var info = _service.GetTemplateByKey(key);
            if (info == null) return null;

            return RichReportTemplate.CreateFromData(info);
        }

        public bool Exists(Guid? key)
        {
            if (key.IsNullOrEmpty()) return false;

            return _service.GetTemplateByKey(key) != null;
        }

        public async Task<bool> ExistsAsync(Guid? key)
        {
            if (key.IsNullOrEmpty()) return false;

            return await _service.ExistsAsync(key);
        }

        public bool IsDuplicate(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            return _service.IsDuplicate(_reportInfo.Id, name);
        }

        public async Task<bool> IsDuplicateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return false;

            return await _service.IsDuplicateAsync(_reportInfo.Id, name);
        }

        public async Task<RichReportTemplateSaveResult> SaveAsync(IRichReportTemplate template, CancellationToken ct = default)
        {
            ArgumentGuardHelper.ArgumentNotNull(template);

            var exists = await ExistsAsync(template.Id);
            if (!exists)
            {
                return await SaveAsAsync(template, template.Name, ct);
            }

            var result = await _service.SaveTemplateAsync(template, ct);

            //result.TryValidateResult();

            return new RichReportTemplateSaveResult(sucessed: result.IsValid, isNew: false, template);
        }

        public async Task<RichReportTemplateSaveResult> SaveAsAsync(IRichReportTemplate template, string newName, CancellationToken ct = default)
        {
            ArgumentGuardHelper.ArgumentNotNull(template);

            var data = template.CreateTemplateData();
            data.Name = newName;

            var result = await _service.SaveAsTemplateAsync(_reportInfo.Id, data, ct);
            //if(!result.TryValidateResult())
            //{
            //    return new RichReportTemplateSaveResult(sucessed: false, isNew: true, null);
            //}

            if (!result.IsValid)
            {
                return new RichReportTemplateSaveResult(sucessed: false, isNew: true, null);
            }

            var addedTemplate = await LoadAsync(result.Value);

            if (addedTemplate == null)
            {
                throw new InvalidOperationException("template not found");
            }
            return new RichReportTemplateSaveResult(sucessed: result.IsValid, isNew: true, addedTemplate);
        }

        public async Task<bool> DeleteAsync(Guid? templateKey)
        {
            if (templateKey.IsNullOrEmpty())
                return false;

            var result = await _service.DeleteTemplateAsync(templateKey);

            //if (result.TryValidateResult())
            //{
            //    return true;
            //}

            if (!result.IsValid)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> RenameAsync(Guid? templateKey, string templateName)
        {
            if (templateKey.IsNullOrEmpty() || string.IsNullOrWhiteSpace(templateName))
                return false;

            var result = await _service.RenameTemplateAsync(templateKey, templateName);

            //if (result.TryValidateResult())
            //{
            //    return true;
            //}

            if (!result.IsValid)
            {
                return false;
            }

            return true;
        }

        public bool Export(IRichReportTemplate template)
            => RichReportTemplateImportExportHelper.ShowExportDialog(template);

        public async Task<IAsyncExecutionResult<IRichReportTemplate>> ImportAsync()
        {
            if (!RichReportTemplateImportExportHelper.TryShowImportDialog(out var template))
            {
                return AsyncExecutionResult<IRichReportTemplate>.AsCanceled();
            }

            if (template == null)
                return AsyncExecutionResult<IRichReportTemplate>.AsCanceled();

            var newName = await GetNewNameAsync();

            template.Id = Guid.NewGuid();
            template.Name = newName;

            return AsyncExecutionResult<IRichReportTemplate>.AsSuccess(template);
        }

        public IRichReportTemplate Import()
        {
            var template = RichReportTemplateImportExportHelper.ShowImportDialog();

            if (template == null)
                return null;

            var newName = GetNewName();

            template.Id = Guid.NewGuid();
            template.Name = newName;

            return template;
        }

        #endregion
    }
}
