using ISoft.Utils.Localization;
using System.Resources;

namespace ISoft.RichReports.Localization
{
    public class RichReportResourceLocalizer : ResourceLocalizerBase<RichReportStringId>
    {
        public const string ResourceFilePath = "ISoft.RichReports.Localization.Resources.ReportResources";

        public RichReportResourceLocalizer()
            : base(new RichReportLocalizer())
        {
            
        }

        protected override ResourceManager CreateResourceManagerCore()
        {
            return new ResourceManager(ResourceFilePath, typeof(RichReportResourceLocalizer).Assembly);
        } 
    }
}
