using ISoft.RichReports.Editor.Fields.Format;

namespace ISoft.RichReports.Editor.Fields.Switches
{
    public class RichFieldFluentSwitchBuilder : IRichFieldFluentSwitchBuilder
    {
        private readonly IRichFieldSwitchBuilder _builder;
        private RichFieldSwitchCollection _switches;

        public RichFieldFluentSwitchBuilder() : this(new RichFieldSwitchBuilder())
        {

        }

        public RichFieldFluentSwitchBuilder(IRichFieldSwitchBuilder builder)
        {
            _builder = builder;
            _switches = new();
        }

        public static IRichFieldFluentSwitchBuilder Create() => new RichFieldFluentSwitchBuilder();

        /// <summary>
        /// Adds a numeric format switch (e.g., \# "#,##0.00") to a field code.
        /// </summary>
        public IRichFieldFluentSwitchBuilder AddNumericFormat(string formatValue)
        {
            if (string.IsNullOrWhiteSpace(formatValue)) return this;
            _switches.AddOrUpdate(_builder.CreateNumericFormatSwitch(formatValue));
            return this;
        }

        /// <summary>
        /// Adds a date/time format switch (e.g., \@ "MMMM d, yyyy") to a field code.
        /// </summary>
        public IRichFieldFluentSwitchBuilder AddDateTimeFormat(string formatValue)
        {
            if (string.IsNullOrWhiteSpace(formatValue)) return this;
            _switches.AddOrUpdate(_builder.CreateDateTimeFormatSwitch(formatValue));
            return this;
        }

        /// <summary>
        /// Adds a general format switch (e.g., \* Upper) to a field code.
        /// </summary>
        public IRichFieldFluentSwitchBuilder AddGeneralFormat(string formatValue)
        {
            if (string.IsNullOrWhiteSpace(formatValue)) return this;
            _switches.AddOrUpdate(_builder.CreateGeneralFormatSwitch(formatValue));
            return this;
        }

        public IRichFieldFluentSwitchBuilder AddDateTimeFormat(RichFieldDateTimeFormatType formatType)
        {
            var format = RichFieldFormatHelper.GetDateTimeFormat(formatType);
            if(format == null) return this;

            return AddFieldFormat(format.Value);
        }

        public IRichFieldFluentSwitchBuilder AddNumericFormat(RichFieldNumericFormatType formatType)
        {
            var format = RichFieldFormatHelper.GetNumericFormat(formatType);
            if (format == null) return this;

            return AddFieldFormat(format.Value);
        }

        public IRichFieldFluentSwitchBuilder AddGeneralFormat(RichFieldGeneralFormatType formatType)
        {
            var format = RichFieldFormatHelper.GetGeneralFormat(formatType);
            if (format == null) return this;

            return AddFieldFormat(format.Value);
        }

        public IRichFieldFluentSwitchBuilder AddFieldFormat(RichFieldFormat format)
        {
            if (RichFieldFormat.IsNullOrEmpty(format)) return this;

            var fieldSwitch = _builder.CreateFormatSwitch(format);
            if (fieldSwitch != null)
            {
                _switches.AddOrUpdate(fieldSwitch);
            }

            return this;
        }

        public IRichFieldFluentSwitchBuilder AddFieldSwitch(string switchCode, string switchValue)
        {
            if (string.IsNullOrWhiteSpace(switchCode)) return this; // allow switchValue if null or empty becouse some switche not require the value
            _switches.AddOrUpdate(_builder.CreateFieldSwitch(switchCode, switchValue)); 
            return this;
        }

        public IRichFieldFluentSwitchBuilder AddFieldSwitch(string switchCode)
        {
            if (string.IsNullOrWhiteSpace(switchCode)) return this;
            _switches.AddOrUpdate(_builder.CreateFieldSwitch(switchCode));
            return this;
        }

        public IRichFieldFluentSwitchBuilder Remove(string switchCode)
        {
            if (string.IsNullOrWhiteSpace(switchCode)) return this;
            _switches.RemoveByKey(switchCode);
            return this;
        }

        public string BuildString()
        {
            if (_switches.Count > 0)
            {
                return _switches.GetBuildString();
            }

            return null;
        }

        public IRichFieldSwitchReadOnlyCollection Build()
        {
            if (_switches.Count > 0)
            {
                return _switches;
            }

            return null;
        }

        //public void End()
        //{
        //    _switches.Clear();
        //    _switches = null;
        //}
    }
}
