namespace ISoft.RichReports.Editor.Fields.Arguments
{
    /// <summary>
    /// Base implementation of a field argument
    /// </summary>
    public class RichFieldArgument : ObjectBase, IRichFieldArgument
    {
        private int _index;
        private string _value;

        public RichFieldArgument()
        {
            _index = -1;
        }

        public RichFieldArgument(string value)
        {
            _value = value;
            _index = -1;
        }

        public RichFieldArgument(int index, string value)
        {
            _index = index;
            _value = value;
        }

        /// <summary>
        /// Gets or sets the index of this argument (0-based)
        /// </summary>
        public int Index
        {
            get => _index;
            private set => _index = value;
        }

        public string Value
        {
            get => _value;
            private set => _value = value;
        }

        public void UpdateIndex(int index)
        {
            Index = index;
        }

        /// <summary>
        /// Gets the parsed value
        /// </summary>
        public T GetValue<T>(T defaultValue = default)
        {
            try
            {
                return Value.TryConvert(defaultValue);
            }
            catch
            {
                return default;
            }
        }

        public string Build()
        {
            if(string.IsNullOrEmpty(Value)) return string.Empty;

            return $" \"{Value}\""; // keep space before

            //string.Format(" \"{0}\"", Value);
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
            return !string.IsNullOrEmpty(Value);
        }

        public IRichFieldArgument Copy()
        {
            return this.CreateClone();
        }

        public override string ToString() => Value ?? string.Empty;

        //public override object Clone()
        //{
        //    return new RichFieldArgument(_index, _value);
        //}
    }
}
