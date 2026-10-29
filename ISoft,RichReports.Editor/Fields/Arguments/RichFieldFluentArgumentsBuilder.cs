namespace ISoft.RichReports.Editor.Fields.Arguments
{
    public class RichFieldFluentArgumentsBuilder : IRichFieldFluentArgumentsBuilder
    {
        private RichFieldArgumentCollection _arguments;

        public RichFieldFluentArgumentsBuilder()
        {
            _arguments = new();
        }

        public static IRichFieldFluentArgumentsBuilder Create() => new RichFieldFluentArgumentsBuilder();

        public IRichFieldFluentArgumentsBuilder Add(string value)
        {
            _arguments.AddValue(value);
            return this;
        }

        public IRichFieldFluentArgumentsBuilder AddOrUpdate(int index, string value)
        {
            _arguments.AddOrUpdateValue(index, value);
            return this;
        }

        public IRichFieldFluentArgumentsBuilder AddValueAt(int index, string value)
        {
            _arguments.AddValueAtIndex(index, value);
            return this;
        }

        public IRichFieldFluentArgumentsBuilder InsertValueAt(int index, string value)
        {
            _arguments.InsertValue(index, value);
            return this;
        }

        public IRichFieldFluentArgumentsBuilder RemoveValueAt(int index)
        {
            _arguments.RemoveValue(index);
            return this;
        }

        public string BuildString()
        {
            var str = GetBuildString();
            Clear();
            return str;
        }

        private string GetBuildString()
        {
            if (_arguments.Count > 0)
            {
                return _arguments.GetBuildString();
            }

            return null;
        }

        private void Clear()
        {
            _arguments.Clear();
            _arguments = null;
        }
    }
}

   

