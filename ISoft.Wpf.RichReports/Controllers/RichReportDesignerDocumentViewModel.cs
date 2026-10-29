//using DevExpress.Mvvm.Native;
//using DevExpress.Mvvm.POCO;
//using ISoft.Mvvm.Extentions;
//using ISoft.Mvvm.Helpers;
//using ISoft.Mvvm.Native;
//using ISoft.Mvvm.ViewModel.Common;
//using ISoft.Printing.Core.Helpers;
//using ISoft.RichReportDesigner.Core.Designer;
//using ISoft.RichReportDesigner.Core.Reports.DataModel;
//using ISoft.RichReportDesigner.Core.Templates;
//using ISoft.RichReportDesigner.UI.Controls;
//using ISoft.RichReportDesigner.UI.DataModel;
//using ISoft.RichReportDesigner.UI.Internal;
//using ISoft.Utils.Enumrations;
//using ISoft.Utils.Extentions;
//using ISoft.Utils.Helpers;
//using ISoft.Utils.Mvvm;
//using System;
//using System.Threading.Tasks;

//namespace ISoft.RichReportDesigner.UI.Controllers
//{
//    public class RichReportDesignerDocumentViewModel : ObjectViewModelBase, IBackstageOwner<RichReportDesignerBackstageTab>
//    {
//        private readonly IBackstageManager<RichReportDesignerBackstageTab> _backstageManager;
//        private readonly IRichReportDesignerControl _designer;

//        private IXpfRichReport Report => _designer.Report;

//        public virtual bool IsBackstageOpened { get; set; }

//        public virtual int BackstageTabIndex { get; set; } = 0;

//        public virtual string DefaultNewTemplateName { get; protected set; }

//        protected RichReportDesignerDocumentViewModel(IRichReportDesignerControl designer)
//        {
//            _designer = designer;
//            _backstageManager = BackstageManager<RichReportDesignerBackstageTab>.Create(this);
//        }

//        public static RichReportDesignerDocumentViewModel Create(IRichReportDesignerControl designerControl)
//           => ViewModelSource.Create(() => new RichReportDesignerDocumentViewModel(designerControl));

//        #region Commands
//        public async Task New()
//        {
//            try
//            {
//                await OpenTemplateCoreAsync(Report.TemplateStorage.CreateNewAsync);
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        public bool CanNew()
//        {
//            return Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowCreateNewTemplate, false);
//        }

//        public async Task OpenDefault(IRichReportTemplatePreviewItem item)
//        {
//            try
//            {
//                await OpenTemplateCoreAsync(async delegate
//                {
//                    return await Report.TemplateStorage.CreateNewAsync(item.Content);
//                });
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        public bool CanOpenDefault(IRichReportTemplatePreviewItem item)
//        {
//            return item != null && Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowCreateNewTemplate, false);
//        }

//        public async Task Open(IRichReportTemplatePreviewItem item)
//        {
//            try
//            {
//                await OpenTemplateCoreAsync(async delegate
//                {
//                    return await Report.TemplateStorage.LoadAsync(item.Id);
//                });
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        public bool CanOpen(IRichReportTemplatePreviewItem item)
//        {
//            return item != null && Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowModefiyTemplate, false);
//        }

//        public async Task Save()
//        {
//            try
//            {
//                await SaveCoreAsync();
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        public bool CanSave()
//        {
//            return _designer.Return(x => x.Controller.TemplateController.CanSave() && Report.PermissionInfo.AllowModefiyTemplate, () => false);
//        }

//        public async Task SaveAs(string name)
//        {
//            try
//            {
//                await SplashScreenManagerService.SaveWithWaitIndicatorAsync(async () =>
//                {
//                    var isDuplicate = await Report.TemplateStorage.IsDuplicateAsync(name);
//                    if (isDuplicate)
//                    {
//                        NotificationManagerHelper.ShowError("إسم النمط موجود بالفعل...يرجى إختيار إسم آخر");
//                        return;
//                    }

//                    var isSaved = await _designer.Controller.TemplateController.SaveAsync(async (t) =>
//                    {
//                        return await Report.TemplateStorage.SaveAsAsync(t, name);
//                    });

//                    if (isSaved)
//                    {
//                        _backstageManager.ShowHideBackstageView(false);
//                    }
//                });
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        public bool CanSaveAs(string name)
//        {
//            return _designer.Return(x => x.Controller.TemplateController.HasTemplate && !string.IsNullOrWhiteSpace(name) && Report.PermissionInfo.AllowCreateNewTemplate, () => false);
//        }

//        public void Export()
//        {
//            try
//            {
//                _designer.Controller.TemplateController.ExecuteWithTemplate((t) => Report.TemplateStorage.Export(t));
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        public bool CanExport()
//        {
//            return _designer.Return(x => x.Controller.TemplateController.HasTemplate && Report.PermissionInfo.AllowExportTemplete, () => false);
//        }

//        public async Task Import()
//        {
//            try
//            {
//                if (!RichReportTemplateImportExportHelper.TryImport(out var template))
//                {
//                    return;
//                }

//                await OpenTemplateCoreAsync(async delegate
//                {
//                    return await Report.TemplateStorage.CreateNewAsync(template.Content);
//                });
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        public bool CanImport()
//        {
//            return _designer.Return(x => x.Controller.TemplateController.HasTemplate && Report.PermissionInfo.AllowImportTemplete, () => false);
//        }

//        public async void TemplateSaved(RichReportDesignerTemplateSavedEventArgs args)
//        {
//            try
//            {
//                DataChangeMessageType changedType = args.IsNew ? DataChangeMessageType.Insert : DataChangeMessageType.Update;
//                await Report.StoreTemplatesSource.OnItemChangedAsync(changedType, args.Template.Id);
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        #endregion

//        #region Actions
//        protected async Task<bool> TrySaveUnsavedChangesAsync()
//        {
//            if (!_designer.Controller.TemplateController.HasChanges())
//                return true;

//            var result = ShowSaveConfirmationDialog();

//            return result switch
//            {
//                ConfirmationResult.Ok => await SaveCoreAsync(),
//                ConfirmationResult.No => true,
//                ConfirmationResult.Cancel => false,
//                _ => false
//            };
//        }

//        protected ConfirmationResult ShowSaveConfirmationDialog()
//           => PrintingTemplateDialogHelper.ShowSaveConfirmationDialog(_designer.ActiveReportTemplate?.Name);

//        public bool CanCloseTemplate() =>
//            !_designer.Controller.TemplateController.HasChanges() || MessageBoxService.ShowEntityCancelConfirmation();

//        protected async Task<bool> SaveCoreAsync()
//        {
//            bool isSaved = false;
//            try
//            {
//                var isNew = IsNewTemplate();
//                if (isNew)
//                {
//                    _backstageManager.OpenTab(RichReportDesignerBackstageTab.SaveAs); // open saveAs tab
//                    DefaultNewTemplateName = _designer.ActiveReportTemplate.Name;
//                    return false;
//                }
//                else
//                {
//                    await SplashScreenManagerService.SaveWithWaitIndicatorAsync(async () =>
//                    {
//                        isSaved = await _designer.Controller.TemplateController.SaveAsync(async (t) =>
//                        {
//                            return await Report.TemplateStorage.SaveAsync(t);
//                        });

//                        if (isSaved)
//                        {
//                            _backstageManager.ShowHideBackstageView(false);
//                        }
//                    });
//                }
//            }
//            catch (Exception ex)
//            {
//                isSaved = false;
//                OnException(ex);
//            }

//            return isSaved;
//        }

//        protected async Task OpenTemplateCoreAsync(Func<Task<IRichReportTemplate>> factory)
//        {
//            try
//            {
//                if (!await TrySaveUnsavedChangesAsync())
//                {
//                    return;
//                }

//                await SplashScreenManagerService.LoadWithWaitIndicatorAsync(async () =>
//                {
//                    var template = await factory();
//                    if (template == null)
//                    {
//                        NotificationManagerHelper.ShowError($"النمط غير موجود");
//                        return;
//                    }

//                    var isLoaded = await _designer.Controller.TemplateController.LoadAsync(template);
//                    if (isLoaded)
//                    {
//                        _backstageManager.ShowHideBackstageView(false);
//                    }
//                }, state: $"جاري فتح التصميم ...");
//            }
//            catch (Exception ex)
//            {
//                OnException(ex);
//            }
//        }

//        private bool IsNewTemplate()
//        {
//            var tempId = _designer.Controller.TemplateController.ActiveTemplate?.Id;
//            if (tempId.IsNullOrEmpty()) throw new InvalidOperationException($"designer ActiveTemplate can't be null");

//            return !_designer.Report.TemplateStorage.Exists(tempId);
//        }
//        #endregion
//    }
//}
