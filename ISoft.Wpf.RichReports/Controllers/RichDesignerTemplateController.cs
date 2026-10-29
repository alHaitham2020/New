using DevExpress.XtraRichEdit;
using ISoft.RichReportDesigner.Core.RichEdit;
using ISoft.RichReportDesigner.Core.RichEdit.Services;
using ISoft.RichReportDesigner.Core.Shared;
using ISoft.RichReportDesigner.Core.Templates;
using ISoft.RichReportDesigner.UI.Controls;
using ISoft.Utils.Data.AsyncData;
using ISoft.Utils.Helpers;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReportDesigner.UI.Controllers
{
    // manage a report templates in the designer
    internal class RichDesignerTemplateController
    {
        private readonly OperationLocker _loadTemplateLocker = new();

        private readonly IRichReportDesignerControl _designer;

        private IRichEditorControl Editor => _designer.Editor;

        public RichDesignerTemplateController(IRichReportDesignerControl designer)
        {
            _designer = designer;
        }

        #region Proprties
        /// <summary>
        /// Gets the current active template.
        /// </summary>
        public IRichReportTemplate ActiveTemplate => _designer.ActiveReportTemplate;

        /// <summary>
        /// Gets whether a template is currently loaded.
        /// </summary>
        public bool HasTemplate => ActiveTemplate != null;

        #endregion

        #region Template Operations

        public void LoadTemplateSource(IRichReportTemplate template)
        => _loadTemplateLocker.DoLockedActionIfNotLocked(() => LoadTemplateCore(template, false));

        /// <summary>
        /// Load a template in the designer.
        /// </summary>
        internal async Task<bool> LoadAsync(IRichReportTemplate template, bool notifyChanged = false)
        {
            return LoadTemplateCore(template: template, notifyChanged: notifyChanged);
        }

        internal bool CanLoad(IRichReportTemplate template) => template != null;

        /// <summary>
        /// Saves the current template.
        /// </summary>
        internal async Task<bool> SaveAsync(Func<IRichReportTemplate, Task<RichReportTemplateSaveResult>> saveFunc)
        {
            return await SaveCoreAsync(saveFunc);
        }

        internal bool HasChanges() => HasTemplate && Editor.Modified;

        internal IRichReportTemplate GetPreviewTemplate()
        {
            if (!HasTemplate) return null;

            return GetCurrentEffectedTemplate(true);
        }

        internal async Task ExecuteWithTemplateAsync(Func<IRichReportTemplate, Task> execute)
        {
            await execute(GetCurrentEffectedTemplate());
        }

        internal void ExecuteWithTemplate(Action<IRichReportTemplate> execute)
        {
            execute(GetCurrentEffectedTemplate());
        }

        internal T ExecuteWithTemplate<T>(Func<IRichReportTemplate,T> execute)
        {
           return execute(GetCurrentEffectedTemplate());
        }

        #endregion

        #region base Methods
        private async Task<bool> SaveCoreAsync(Func<IRichReportTemplate, Task<RichReportTemplateSaveResult>> storage)
        {
            try
            {
                if (!HasTemplate) return false;

                var template = GetCurrentEffectedTemplate();

                var result = await storage(template);

                if (result.Sucessed)
                {
                    CommitChanges();

                    var isNew = result.IsNew;
                    var newTemplate = result.Template;

                    OnTemplateSaved(newTemplate, isNew);

                    if (isNew)
                    {
                        LoadTemplateCore(newTemplate);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                ShowError("Error saving template", ex);
                return false;
            }
        }

        private bool LoadTemplateCore(IRichReportTemplate template, bool notifyChanged = false)
        {
            try
            {
                bool isLoaded = LoadDocumentOrCreateNew(template?.Content);
                if (!isLoaded) return false;

                OnTemplateChanged(template);// must called before update the selectedTemplate
                SetChanged(notifyChanged);

                return true;
            }
            catch (Exception ex)
            {
                ShowError($"Error loading template: {template.Name}", ex);
                return false;
            }
        }

        private IRichReportTemplate GetCurrentEffectedTemplate(bool isPreview = false)
        {
            EnsureActiveTemplate();

            IRichReportTemplate template = ActiveTemplate.CreateClone();

            if (!isPreview)
            {
                SaveTemplateProperties(template);
            }

            SaveTemplateContent(template);

            if (!isPreview)
            {
                SaveTemplateThumbinal(template);
            }

            return template;
        }

        private bool LoadDocumentOrCreateNew(byte[] content)
        {
            if (content == null)
            {
                return Editor.CreateNewDocument(false);
            }

            return LoadTemplateContent(content);
        }

        private bool LoadTemplateContent(byte[] content)
        {
            return Editor.LoadDocumentTemplate(content);
        }

        private void SaveTemplateContent(IRichReportTemplate template)
        {
            template.UpdateContent(Editor.SaveDocument(DocumentFormat.Rtf));
        }

        private void SaveTemplateThumbinal(IRichReportTemplate template)
        {
            template.UpdateThumbnail(Editor.ExportToImage());
        }

        private void SaveTemplateProperties(IRichReportTemplate template)
        {
            _designer
                .GetService<IRichDocumentPropertiesManagerService>()
                .UpdateDocDesignProperties(Editor.Document.DocumentProperties, template);
        }

        private void CommitChanges()
        {
            SetChanged(false);
        }

        internal void MarkAsChanged()
        {
            SetChanged(true);
        }

        private void SetChanged(bool change)
        {
            Editor.Modified = change;
        }

        private void EnsureActiveTemplate()
        {
            if (!HasTemplate)
            {
                throw new ArgumentNullException(nameof(ActiveTemplate));
            }
        }

        #endregion

        #region Event Invokers

        private void OnTemplateChanged(IRichReportTemplate template)
        {
            if (ActiveTemplate == null || !DataValidationHelper.EqualsTo(template?.Id, ActiveTemplate?.Id))
            {
                _designer.RaiseActiveTemplateChanged(template);
            }
        }

        private void OnTemplateSaved(IRichReportTemplate template, bool isNew)
        {
            _designer.RaiseTemplateSaved(template, isNew);
        }

        #endregion

        #region Error Handling

        private void ShowError(string message, Exception ex) => throw new RichOperationException(message, ex);

        #endregion
    }
}
