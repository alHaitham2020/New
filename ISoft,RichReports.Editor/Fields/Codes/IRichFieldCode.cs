using ISoft.RichReports.Editor.Fields.Arguments;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Switches;

namespace ISoft.RichReports.Editor.Fields.Codes
{
    public interface IRichFieldCode
    {
        RichEditFieldInfo FieldInfo { get; }

        RichEditFieldType FieldType { get;}

        string FieldName { get; }

        string FieldProperty { get; }

        RichEditFieldKey FieldKey { get; }

        bool IsTableStart { get; }

        bool IsTableEnd { get; }

        bool IsRegion { get; }

        bool IsDataTableField { get;}

        string OriginalCode { get;}

        IRichFieldArgumentCollection Arguments { get; }

        IRichFieldSwitchCollection Switches { get; }

        string GetDataTableName();

        string Build();

        bool TryBuild(out string result);

        bool IsValid();

        void Validate();

        IRichFieldCode Copy();
    }
}
