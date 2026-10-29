using ISoft.Utils.Common;
using ISoft.Utils.Extentions;

namespace ISoft.RichReports.Editor.Fields.Argumentes
{
    public sealed class RichFieldArgumentStringBuilder : IRichFieldArgumentBuilder
    {
        private readonly FluentStringBuilder stringBuilder;

        public RichFieldArgumentStringBuilder()
        {
            stringBuilder = new FluentStringBuilder();
        }

        public static IRichFieldArgumentBuilder CreateNew()
        {
            return new RichFieldArgumentStringBuilder();
        }

        public IRichFieldArgumentBuilder Add(object value)
        {
            if (value.IsNullStringOrEmpty()) return this;

            stringBuilder.AppendFormat(" \"{0}\"", value);
            return this;
        }

        public string Build()
        {
            return stringBuilder.ToString();
        }
    }
}

   

