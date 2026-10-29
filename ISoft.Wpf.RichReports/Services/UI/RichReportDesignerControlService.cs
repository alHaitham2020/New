using DevExpress.Mvvm.UI;
using ISoft.RichReportDesigner.UI.Controls;

namespace ISoft.RichReportDesigner.UI.UIServices
{
    public class RichReportDesignerControlService : ServiceBaseGeneric<RichReportDesignerControl>, IRichReportDesignerControlService
    {
        #region IRichReportDesignerControlService
        //IRichReportTemplate IRichReportDesignerControlService.ActiveTemplate => AssociatedObject.ActiveReportTemplate;

        //bool IRichReportDesignerControlService.HasTemplate => AssociatedObject.Controller.TemplateController.HasTemplate;

        bool IRichReportDesignerControlService.NeedsSave() => AssociatedObject.CanSave();

        //bool IRichReportDesignerControlService.CanLoad(IRichReportTemplate template) => AssociatedObject.Controller.TemplateController.CanLoad(template);

        //bool IRichReportDesignerControlService.CanSave() => AssociatedObject.Controller.TemplateController.CanSave();

        //Task<bool> IRichReportDesignerControlService.LoadTemplateAsync(IRichReportTemplate template) => AssociatedObject.Controller.TemplateController.LoadAsync(template);

        //Task<bool> IRichReportDesignerControlService.SaveTemplateAsync(Func<IRichReportTemplate, Task<RichReportTemplateSaveResult>> saveFunc) => AssociatedObject.Controller.TemplateController.SaveAsync(saveFunc);

        //Task IRichReportDesignerControlService.ExecuteWithTemplateAsync(Func<IRichReportTemplate, Task> execute) => AssociatedObject.Controller.TemplateController.ExecuteWithTemplateAsync(execute);

        //void IRichReportDesignerControlService.ExecuteWithTemplate(Action<IRichReportTemplate> execute) => AssociatedObject.Controller.TemplateController.ExecuteWithTemplate(execute);

        #endregion
    }
}
