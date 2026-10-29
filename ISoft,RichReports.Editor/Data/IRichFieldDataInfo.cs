using System;

namespace ISoft.RichReports.Editor.Data
{
    public interface IRichFieldDataInfo
    {
        string Name { get; }

        string DisplayName { get; }

        string Path { get; }

        Type DataType { get; }

        object NullValue { get; }

        bool IsImageField { get; }

        ImageUriSchemeType ImageSchemeType { get;}

        bool IsDetailField { get; }

        bool AllowUseDefaultEmptyValue { get;  }
    }
}
