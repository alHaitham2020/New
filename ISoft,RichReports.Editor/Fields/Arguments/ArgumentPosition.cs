namespace ISoft.RichReports.Editor.Fields.Argumentes
{
    /// <summary>
    /// Represents an argument's position in the original string.
    /// </summary>
    public class ArgumentPosition
    {
        public int Index { get; set; }

        public string Value { get; set; }

        public int StartIndex { get; set; }

        public int Length { get; set; }

        public int EndIndex { get; set; }

        public override string ToString()
        {
            return $"Index : {Index} Value :{Value}";
        }
    }
}
