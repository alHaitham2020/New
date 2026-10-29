using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.DataModel;
using ISoft.Utils.Helpers;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    /// <summary>
    /// Represents a field switch (e.g., \* Upper, \# $#,##0.00)
    /// </summary>
    public class RichFieldSwitch : ObjectBase, IRichFieldSwitch
    {
        #region Fields
        private readonly RichSwitchType _switchType;
        private readonly string _switchKey;
        private readonly string _switchValue;
        #endregion

        public RichFieldSwitch(RichSwitchType switchType, string switchKey, string switchValue)
        {
            _switchType = switchType;
            _switchKey = switchKey;
            _switchValue = switchValue;
        }

        #region Proprties
        public RichSwitchType SwitchType => _switchType;

        public string SwitchKey => _switchKey;

        public string SwitchValue => _switchValue;

        public bool IsFormatSwitch => _switchType != RichSwitchType.None && _switchType != RichSwitchType.FieldSpecific;

        public bool IsDateTimeFormatSwitch => _switchType == RichSwitchType.DateTime;

        public bool IsNumericFormatSwitch => _switchType == RichSwitchType.Numeric;

        #endregion

        #region Actions
        /// <summary>
        /// Builds the switch string
        /// </summary>
        public string Build()
        {
            return RichFieldSwitchCodeHelper.Build(SwitchKey, SwitchValue);
        }

        public bool TryBuild(out string result)
        {
            if (!IsValid())
            {
                result = string.Empty;
                return false;
            }

            result = Build();
            return true;
        }

        public bool IsValid()
        {
            return SwitchType != RichSwitchType.None && !string.IsNullOrWhiteSpace(SwitchKey);
        }

        /// <summary>
        /// Gets the parsed value
        /// </summary>
        public T GetValue<T>(T defaultValue = default)
        {
            try
            {
                return SwitchValue.TryConvert(defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        public override string ToString()
        {
            return Build();
        }

        IRichFieldSwitch IRichFieldSwitch.Copy() => this.CreateClone();
        #endregion

        #region Factory

        public static RichFieldSwitch CreateSwitch(string switchKey, string switchValue)
        {
            var switchType = RichFieldSwitchCodeHelper.GetSwitchType(switchKey);
            return new RichFieldSwitch(switchType, switchKey, switchValue);
        }

        public static RichFieldSwitch CreateFieldSpecific(string switchKey, string switchValue)
        {
            return new RichFieldSwitch(RichSwitchType.FieldSpecific, switchKey, switchValue);
        }

        public static RichFieldSwitch CreateGeneralFormat(string formatValue)
        {
            return new RichFieldSwitch(RichSwitchType.General, RichStandardFieldSwitchKeys.GeneralKey, formatValue);
        }

        public static RichFieldSwitch CreateDateTimeFormat(string formatValue)
        {
            return new RichFieldSwitch(RichSwitchType.DateTime, RichStandardFieldSwitchKeys.DateTimeKey, formatValue);
        }

        public static RichFieldSwitch CreateNumericFormat(string formatValue)
        {
            return new RichFieldSwitch(RichSwitchType.Numeric, RichStandardFieldSwitchKeys.NumericKey, formatValue);
        }
        #endregion
    }
}
