using ISoft.Office.Images;
using ISoft.RichReports.Data.Fields;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.Enumrations;

namespace ISoft.RichReports.Fields
{
    public interface IRichFieldNode : IRichNode
    {
        IRichFieldGroup ParentGroup { get;}

        string FieldProperty { get;}

        string DataMember { get; }

        RichFieldFormat? FieldFormat { get; }

        object NullValue { get;}

        RichFieldDataTypeCategory DataTypeCategory { get; }

        DataTypeKind DataTypeKind { get; }

        ImageUriSchemeType ImageSchemeType { get; }

        bool IsDetailField { get; }

        bool HasFormat { get; }

        bool IsImageField { get; }
    }
}
