using DevExpress.Mvvm.POCO;
using ISoft.Mvvm.Extentions;
using ISoft.Mvvm.Helpers;
using ISoft.RichReports.Core.Client;
using ISoft.RichReports.Core.Model;
using ISoft.RichReports.Core.Templates;
using ISoft.RichReports.Editor.Services;
using ISoft.RichReports.UI;
using ISoft.Utils.Data;
using ISoft.Utils.Helpers;
using ISoft.Wpf.RichReports.Services;
using System;
using System.Threading.Tasks;

namespace ISoft.Wpf.RichReports.DataModel
{
    /// <summary>
    /// a rich report for desktop wpf platform
    /// </summary>
    public class XpfRichReport : RichReportBase, IXpfRichReport
    {
        #region Constructors
        public XpfRichReport(IRichReportContext context) : base(context)
        {

        }
        #endregion

        #region Fields
        private IRichReportTemplate _designerTemplate;
        #endregion

        #region Proprties
        public IXpfRichReportTemplateItemsSource DefaultTemplatesSource { get; private set; }

        public IXpfRichReportTemplateItemsSource StoreTemplatesSource { get; private set; }

        public IRichReportTemplate DesignerTemplate
        {
            get
            {
                return _designerTemplate;
            }
            private set
            {
                _designerTemplate = value;
            }
        }

        #endregion

        #region Actions
        public async Task NewTemplateAsync() =>
            await ExecuteTemplateOperationAsync(
                () => ReportContext.TemplateStorage.CreateNewAsync(),
                "Failed to create a new template");

        public async Task CloneAsync(Guid? templateKey) =>
            await ExecuteTemplateOperationAsync(
                () => ReportContext.TemplateStorage.CreateFromAsync(templateKey),
                "Failed to clone template");

        public async Task EditTemplateAsync(Guid? templateKey)
        {
            var result = await LoadTemplateAsync(templateKey);
            if (!HandelResult(result)) return;

            ShowDesignerCore(result.Value);
        }

        public void EditTemplate(IRichReportTemplate template)
        {
            ArgumentGuardHelper.ArgumentNotNull(template, nameof(template));
            ShowDesignerCore(template);
        }

        public void ShowTemplatesEditorDialog(string title = null)
        {
            XpfRichReportHelper.ShowTemplatesEditorDialog(this, title ?? ReportContext.ReportInfo.Title);
        }

        private async Task ExecuteTemplateOperationAsync(Func<Task<IRichReportTemplate>> operation, string errorMessage)
        {
            try
            {
                var template = await operation();
                if (template == null)
                {
                    NotificationManagerHelper.ShowError("النمط لا يمكن أن يكون فارغاً");
                    return;
                }

                ShowDesignerCore(template);
            }
            catch (Exception ex)
            {
                throw new RichOperationException(errorMessage, ex);
            }
        }

        private void ShowDesignerCore(IRichReportTemplate template)
        {
            ArgumentGuardHelper.ArgumentNotNull(template, nameof(template));

            DesignerTemplate = template;
            XpfRichReportHelper.ShowReportDesignerDocument(this);
        }

        private void CreateTemplateSources()
        {
            var defaultTemplateInfoProvider = ReportContext.DefaultTemplateInfoProvider;

            if (defaultTemplateInfoProvider != null)
            {
                DefaultTemplatesSource = RichDefaultReportTemplateItemsSource.Create(
                defaultTemplateInfoProvider,
                ReportContext.ReportInfo);
            }

            var templateInfoProviderService = ReportContext.TemplateInfoProvider;
            if (templateInfoProviderService != null)
            {
                StoreTemplatesSource = RichReportTemplateItemsSource.Create(
                    templateInfoProviderService,
                    ReportContext.ReportInfo,
                    RichReportTemplateSourceType.PrintTemplates);
            }
        }

        #endregion

        #region Ovverieds

        protected override void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();

            if (!this.IsInDesignMode())
            {
                CreateTemplateSources();
            }
        }

        protected override void RegisterServices()
        {
            base.RegisterServices();
            AddService(typeof(IRichWaitIndicationService), new RichWaitIndicationService());
        }

        protected override bool HandelResult(DataValidationResult validationResult)
        {
            return validationResult.TryValidateResult();
        }

        #endregion
    }
}
