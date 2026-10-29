namespace ISoft.Wpf.RichReports.Services.UI
{
    public interface IRichReportDesignerControlService
    {
        //IRichReportTemplate ActiveTemplate { get; }

        //bool HasTemplate { get; }

        bool NeedsSave();

        //bool CanLoad(IRichReportTemplate template);

        //bool CanSave();

        //Task<bool> LoadTemplateAsync(IRichReportTemplate template);

        //Task<bool> SaveTemplateAsync(Func<IRichReportTemplate, Task<RichReportTemplateSaveResult>> storage);

        //Task ExecuteWithTemplateAsync(Func<IRichReportTemplate, Task> execute);

        //void ExecuteWithTemplate(Action<IRichReportTemplate> execute);
    }
}
