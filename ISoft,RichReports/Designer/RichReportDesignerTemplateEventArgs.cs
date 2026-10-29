using System;

namespace ISoft.RichReports.Designer
{
    public class RichReportDesignerTemplateEventArgs : EventArgs
    {
        private readonly IRichReportTemplate template;

        public IRichReportTemplate Template => template;

        public RichReportDesignerTemplateEventArgs(IRichReportTemplate template)
        {
            this.template = template;
        }
    }
}
