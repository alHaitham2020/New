using ISoft.RichReports.Editor.Fields.Format;
using System;

namespace ISoft.RichReports.Editor.RichProperties
{
    public interface IRichProperty : ICloneable
    {
        string Name { get; }

        string DisplayName { get; }

        object DefaultValue { get; }

        bool AllowUseDefaultEmptyValue {  get; }

        bool IsVisible { get; }

        Type PropertyType { get; }

        RichFieldFormat? Format { get; }

        bool HasFormat { get; }
    }
}
