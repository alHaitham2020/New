using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using ISoft.Utils.Localization;
using System;

namespace ISoft.RichReports.Localization
{
    public class RichReportLocalizer : LocalizerBase<RichReportStringId>
    {
        static RichReportLocalizer()
        {
            SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<RichReportStringId>(CreateDefaultLocalizer()));
        }

        public RichReportLocalizer()
        {
           
        }

        #region Ovverides
        public override XtraLocalizer<RichReportStringId> CreateResXLocalizer()
        {
            return new RichReportResourceLocalizer();
        }
        #endregion

        #region Static
        public static XtraLocalizer<RichReportStringId> CreateDefaultLocalizer()
        {
            return new RichReportResourceLocalizer();
        }

        // Returns the active UILocalizer instance thats will be used as UI Binding Source for Localization
        public static ILocalizer GetActiveLocalizer()
        {
            return (ILocalizer)(object)Active;
        }

        public static string GetString(RichReportStringId id)
        {
            return Active.GetLocalizedString(id);
        }

        public static string GetLocalizedString(string stringId)
        {
            return GetString((RichReportStringId)Enum.Parse(typeof(RichReportStringId), stringId, ignoreCase: false));
        }

        #endregion
    }
}
