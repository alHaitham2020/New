using DevExpress.Mvvm.DataAnnotations;
using DevExpress.XtraPrinting;
using ISoft.Mvvm.ViewModel.Common;
using ISoft.RichReportDesigner.UI.DataModel;
using ISoft.Utils.Extentions;
using ISoft.Utils.Helpers;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReportDesigner.UI.ViewModel
{
    /// <summary>
    /// ViewModel for template selection dialog
    /// </summary>
    public class RichReportTemplatesEditorDialogViewModel : DocumentViewModelBase
    {
        public virtual IXpfRichReport Report { get; set; }

        public virtual Guid? SelectedTemplateItemKey { get; set; }

        public RichReportTemplatesEditorDialogViewModel(IXpfRichReport report)
        {
            Report = report;
        }

        #region Commands
        public async Task New()
        {
            try
            {
                await Report.NewTemplateAsync();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public bool CanNew()
        {
            return Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowCreateNewTemplate, false);
        }

        public async Task Clone()
        {
            try
            {
                await Report.CloneAsync(SelectedTemplateItemKey);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public bool CanClone()
        {
            return CanExecuteTemplateItemCommand() && Report.ReturnValueOrDefault(x => x.PermissionInfo.AllowCreateNewTemplate, false);
        }

        [Command(CanExecuteMethodName = nameof(CanExecuteTemplateItemCommand))]
        public async Task Edit()
        {
            try
            {
                await Report.EditTemplateAsync(SelectedTemplateItemKey);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        [Command(CanExecuteMethodName = nameof(CanExecuteTemplateItemCommand))]
        public async Task Print()
        {
            try
            {
                await Report.PrintAsync(SelectedTemplateItemKey);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        [Command(CanExecuteMethodName = nameof(CanExecuteTemplateItemCommand))]
        public async Task PrintDirect()
        {
            try
            {
                await Report.PrintDirectAsync(SelectedTemplateItemKey);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        [Command(CanExecuteMethodName = nameof(CanExecuteTemplateItemCommand))]
        public async Task ShowPreview()
        {
            try
            {
                await Report.ShowPreviewAsync(SelectedTemplateItemKey);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public async Task Export(ExportFormat format)
        {
            try
            {
                await Report.ExportAsync(SelectedTemplateItemKey, format);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public bool CanExport(ExportFormat format)
        {
            return CanExecuteTemplateItemCommand();
        }

        public void Refresh()
        {
            try
            {
                Report.StoreTemplatesSource.Refresh();
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public bool CanRefresh()
        {
            return Report != null;
        }

        public bool CanExecuteTemplateItemCommand()
        {
            return !SelectedTemplateItemKey.IsNullOrEmpty();
        }
        #endregion
    }
}
