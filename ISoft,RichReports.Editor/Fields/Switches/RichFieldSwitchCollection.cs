using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.Caching;
using ISoft.Utils.Helpers;
using System;
using System.Linq;
using System.Text;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public class RichFieldSwitchCollection : UniqueItemsCacheBase<string, IRichFieldSwitch>, IRichFieldSwitchCollection
    {
        private static readonly Func<IRichFieldSwitch, string> _selector = (x) => x.SwitchKey;

        public RichFieldSwitchCollection() : base(_selector, StringComparer.OrdinalIgnoreCase)
        {
            
        }

        protected override IRichFieldSwitch CloneItem(IRichFieldSwitch item)
        {
            return item.Copy();
        }

        public void AddOrUpdateSwitch(string switchKey, string switchValue)
        {
            ArgumentGuardHelper.ArgumentIsNotNullOrEmpty(switchKey, nameof(switchKey));
            AddOrUpdate(RichFieldSwitch.CreateFieldSpecific(switchKey, switchValue));
        }

        public bool TryUpdateSwitch(string switchKey, string switchValue)
        {
           return TryUpdate(RichFieldSwitch.CreateFieldSpecific(switchKey, switchValue));
        }

        public bool TryGetFormat(out RichFieldFormat? resultFormat)
        {
            var formatSwitch = Values.SingleOrDefault(x => x.IsFormatSwitch);
            resultFormat = formatSwitch?.GetFormat();
            return resultFormat is not null;
        }

        public string GetSwitchValue(string switchKey)
        {
            if (TryGetItem(switchKey, out var sw))
            {
                return sw.SwitchValue;
            }
            return null;
        }

        public T GetSwitchValue<T>(string switchKey, T defaultValue = default)
        {
            if (TryGetItem(switchKey, out var sw))
            {
                return sw.GetValue(defaultValue);
            }

            return defaultValue;
        }

        /// <summary>
        /// Removes all switches of a specific format type from a field.
        /// </summary>
        /// <param name="field">Field to modify</param>
        /// <param name="switchType">Format switch type to remove</param>
        public void RemoveTypeSwitches(RichSwitchType switchType)
        {
            RemoveItems(i => i.Value.SwitchType == switchType);
        }

        public string GetBuildString()
        {
            var sb = new StringBuilder();

            BuildTo(sb);

            return sb.ToString();
        }

        public void BuildTo(StringBuilder sb)
        {
            var switches = Values;
            foreach (var fs in switches)
            {
                //sb.Append(' ').Append(sw.Build());

                //var item = GetItemByKey(k);
                //if (item == null) continue;

                if (fs.TryBuild(out var code))
                    sb.Append(code);
            }
        }
    }
}
