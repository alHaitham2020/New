using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public interface IRichDataFieldsBuildOptions
    {
        bool AllowCaching { get;}

        HashSet<Type> ExcludedTypes { get;}

        bool IncludeSourcePath { get;}

        int MaxDepth { get;}

        bool AllowUseDefaultFormat { get;}

        bool AllowUseDefaultEmptyValue { get;}

        IRichDataFieldsBuildOptions Clone();
    }
}
