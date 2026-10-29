namespace ISoft.RichReports.Core.Templates
{
    public class RichReportTemplateSaveResult
    {
        public bool Sucessed { get; }

        public bool IsNew { get; }

        public IRichReportTemplate Template { get; }

        public RichReportTemplateSaveResult(bool sucessed, bool isNew, IRichReportTemplate template = null)
        {
            Sucessed = sucessed;
            IsNew = isNew;
            Template = template;
        }
    }
}
