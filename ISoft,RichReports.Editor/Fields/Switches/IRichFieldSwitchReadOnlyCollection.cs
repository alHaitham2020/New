using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.Caching;
using System.Text;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public interface IRichFieldSwitchReadOnlyCollection : IUniqueReadOnlyItemsCache<string, IRichFieldSwitch>
    {
        string GetSwitchValue(string switchKey);

        T GetSwitchValue<T>(string switchKey, T defaultValue = default);

        bool TryGetFormat(out RichFieldFormat? resultFormat);

        string GetBuildString();

        void BuildTo(StringBuilder sb);
    }
}
