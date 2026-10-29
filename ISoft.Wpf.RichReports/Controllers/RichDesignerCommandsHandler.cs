using ISoft.Wpf.RichReports.Controls;

namespace ISoft.Wpf.RichReports.Controllers
{
    internal sealed class RichDesignerCommandsHandler
    {
        private readonly IRichReportDesignerControl _designer;
        private readonly RichDesignerTemplateController _templateController;

        public RichDesignerCommandsHandler(IRichReportDesignerControl designerControl, RichDesignerTemplateController templateController)
        {
            _designer = designerControl ?? throw new ArgumentNullException(nameof(designerControl));
            _templateController = templateController ?? throw new ArgumentNullException(nameof(templateController));
        }

        private IXpfRichReport Report => _designer.Report;

        #region Template Commands
        internal async Task NewAsync()
        {
            try
            {
                await OpenTemplateCoreAsync(Report.TemplateStorage.CreateNewAsync);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanNew()
        {
            return Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowCreateNewTemplate, false);
        }

        internal async Task OpenAsync(byte[] templateContent)
        {
            try
            {
                await OpenTemplateCoreAsync(async delegate
                {
                    return await Report.TemplateStorage.CreateNewAsync(templateContent);
                });
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal async Task OpenDefaultAsync(IRichReportTemplatePreviewItem item)
        {
            try
            {
                await OpenTemplateCoreAsync(async delegate
                {
                    return await Report.TemplateStorage.CreateNewAsync(item.Content);
                });
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanOpenDefault(IRichReportTemplatePreviewItem item)
        {
            return item != null && Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowCreateNewTemplate, false);
        }

        internal async Task OpenAsync(IRichReportTemplatePreviewItem item)
        {
            try
            {
                await OpenTemplateCoreAsync(async delegate
                {
                    return await Report.TemplateStorage.LoadAsync(item.Id);
                });
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanOpen(IRichReportTemplatePreviewItem item)
        {
            return item != null && Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowModefiyTemplate, false);
        }

        internal async Task SaveAsync()
        {
            try
            {
                await SaveCoreAsync();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanSave()
        {
            return Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowModefiyTemplate && _templateController.HasChanges(), false);
        }

        internal async Task SaveAsAsync(string name)
        {
            try
            {
                await MainSplashScreenManagerHelper.SaveWithWaitIndicatorAsync(async () =>
                {
                    var isDuplicate = await Report.TemplateStorage.IsDuplicateAsync(name);
                    if (isDuplicate)
                    {
                        NotificationManagerHelper.ShowError("إسم النمط موجود بالفعل...يرجى إختيار إسم آخر");
                        return;
                    }

                    var isSaved = await _templateController.SaveAsync(async (t) =>
                    {
                        return await Report.TemplateStorage.SaveAsAsync(t, name);
                    });

                    if (isSaved)
                    {
                        _designer.BackstageManager.ShowHideBackstageView(false);
                    }
                });
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanSaveAs(string name)
        {
            return Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowCreateNewTemplate && _templateController.HasTemplate && !string.IsNullOrWhiteSpace(name), false);
        }

        internal void Export()
        {
            try
            {
                if (!CanExport()) return;

                var isExported = _templateController.ExecuteWithTemplate(Report.TemplateStorage.Export);
                if (isExported)
                {
                    NotificationManagerHelper.ShowSuccess("تم التصدير بنجاح");
                }
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanExport()
        {
            return Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowExportTemplete && _templateController.HasTemplate, false);
        }

        internal async Task ImportAsync()
        {
            try
            {
                if (!CanImport()) return;

                if (!RichReportTemplateImportExportHelper.TryShowImportDialog(out var template))
                {
                    return;
                }

                await OpenTemplateCoreAsync(factory: async delegate
                {
                    return await Report
                                .TemplateStorage
                                .CreateNewAsync(template.Content);

                }, notifyChanged: true);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanImport()
        {
            return Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowImportTemplete && _templateController.HasTemplate, false);
        }

        internal async Task ShowPrintPreviewAsync()
        {
            try
            {
                var template = _templateController.GetPreviewTemplate();
                if (template == null) return;

                await Report.ShowPrintPreviewAsync(template);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        internal bool CanShowPreview()
        {
            return _templateController.HasTemplate;
        }

        #endregion

        #region Actions
        private async Task<bool> TrySaveUnsavedChangesAsync()
        {
            if (!_templateController.HasChanges())
                return true;

            var result = ShowSaveConfirmationDialog();

            return result switch
            {
                ConfirmationResult.Ok => await SaveCoreAsync(),
                ConfirmationResult.No => true,
                ConfirmationResult.Cancel => false,
                _ => false
            };
        }

        private ConfirmationResult ShowSaveConfirmationDialog()
           => PrintingTemplateDialogHelper.ShowSaveConfirmationDialog(_templateController.ActiveTemplate?.Name);

        private async Task<bool> SaveCoreAsync()
        {
            bool isSaved = false;
            try
            {
                var isNew = IsNewTemplate();
                if (isNew)
                {
                    _designer.BackstageManager.OpenTab(RichReportDesignerBackstageTab.SaveAs); // open saveAs tab
                    _designer.DefaultNewTemplateName = _templateController.ActiveTemplate?.Name;
                    return false;
                }
                else
                {
                    await _designer.GetService<IRichWaitIndicationService>().ShowWaitIndicatorAsync(async () =>
                    {
                        isSaved = await _templateController.SaveAsync(async (t) =>
                        {
                            return await Report.TemplateStorage.SaveAsync(t);
                        });

                        if (isSaved)
                        {
                            _designer.BackstageManager.ShowHideBackstageView(false);
                        }
                    }, SharedUILocalizer.IndicatorState_Saving);
                }
            }
            catch (Exception ex)
            {
                isSaved = false;
                OnException(ex);
            }

            return isSaved;
        }

        private async Task OpenTemplateCoreAsync(Func<Task<IRichReportTemplate>> factory, bool notifyChanged = false)
        {
            try
            {
                if (!await TrySaveUnsavedChangesAsync())
                {
                    return;
                }

                await _designer.GetService<IRichWaitIndicationService>()
                               .ShowWaitIndicatorAsync(async () =>
                               {
                                   var template = await factory();
                                   if (template == null)
                                   {
                                       NotificationManagerHelper.ShowError($"النمط غير موجود");
                                       return;
                                   }

                                   var isLoaded = await _templateController.LoadAsync(template, notifyChanged);
                                   if (isLoaded)
                                   {
                                       _designer.BackstageManager.ShowHideBackstageView(false);
                                   }

                               }, state: $"جاري فتح التصميم ...");
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        private bool IsNewTemplate()
        {
            var tempId = _templateController.ActiveTemplate?.Id;
            if (tempId.IsNullOrEmpty()) throw new InvalidOperationException($"designer ActiveTemplate can't be null");

            return !Report.TemplateStorage.Exists(tempId);
        }

        private async Task<bool> IsNewTemplateAsync()
        {
            var tempId = _templateController.ActiveTemplate?.Id;
            if (tempId.IsNullOrEmpty()) throw new InvalidOperationException($"designer ActiveTemplate can't be null");

            return !await Report.TemplateStorage.ExistsAsync(tempId);
        }

        private void OnException(Exception ex)
        {
            _designer.HandleException(ex);
        }

        #endregion
    }
}
