using ISoft.RichReports.Editor.Fields.Core;
using ISoft.Utils.Localization;

namespace ISoft.RichReports.Editor.Localization
{
    public class RichFieldTypeNamesLocalizer : ResourceManagerLocalizerBase<RichEditFieldType>
    {
        public const string ResourceFilePath = "ISoft.RichReports.Editor.Localization.Resources.RichFieldTypeNameResources";

        protected override string ResourceBaseName => ResourceFilePath;

        public static readonly IResourceManagerLocalizer<RichEditFieldType> Current = new RichFieldTypeNamesLocalizer();

        public RichFieldTypeNamesLocalizer()
        {
            
        }

        public static string Get(RichEditFieldType id)
        {
            return Current.GetLocalizedString(id);
        }
    }
}
