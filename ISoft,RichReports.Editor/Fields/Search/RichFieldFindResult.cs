using ISoft_RichReports.Editor.Fields.Codes;

namespace ISoft_RichReports.Editor.Fields.Search
{
    public class RichFieldFindResult : IRichFieldFindResult
    {
        public RichFieldFindResult(IRichFieldCode fieldCode, Field field)
        {
            FieldCode = fieldCode;
            Field = field;
        }

        public IRichFieldCode FieldCode { get; }

        public Field Field { get; }
    }
}
