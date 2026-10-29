using DevExpress.XtraRichEdit.API.Native;
using ISoft.RichReports.Editor.Fields.Codes;
namespace ISoft.RichReports.Editor.Fields.Search
{
    public interface IRichFieldFindResult
    {
        IRichFieldCode FieldCode { get; }

        Field Field { get; }
    }
}
