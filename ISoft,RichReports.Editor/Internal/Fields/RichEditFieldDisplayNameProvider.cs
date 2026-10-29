using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Localization;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public interface IRichEditFieldDisplayNameProvider
    {
        string GetDisplayName(RichEditFieldType fieldType);
    }

    public class RichEditFieldDisplayNameProvider : IRichEditFieldDisplayNameProvider
    {
        public string GetDisplayName(RichEditFieldType fieldType)
        {
            return RichFieldTypeNamesLocalizer.Get(fieldType);
        }
    }
}
