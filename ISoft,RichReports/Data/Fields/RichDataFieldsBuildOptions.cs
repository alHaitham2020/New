using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public class RichDataFieldsBuildOptions : IRichDataFieldsBuildOptions
    {
        private static readonly HashSet<Type> _defaultExcludedTypes = [typeof(Guid)];

        public bool IncludeSourcePath { get; set; } = true;

        /// <summary>
        /// Max nested depth
        /// </summary>
        public int MaxDepth { get; set; } = 2;

        public bool AllowCaching { get; set; } = true;

        public bool AllowUseDefaultEmptyValue { get; set; } = true;

        public bool AllowUseDefaultFormat { get; set; } = true;

        public HashSet<Type> ExcludedTypes { get; set; } = [.. _defaultExcludedTypes];

        public RichDataFieldsBuildOptions Clone()
        {
            return new RichDataFieldsBuildOptions
            {
                IncludeSourcePath = IncludeSourcePath,
                MaxDepth = MaxDepth,
                AllowCaching = AllowCaching,
                ExcludedTypes = [.. ExcludedTypes]
            };
        }

        IRichDataFieldsBuildOptions IRichDataFieldsBuildOptions.Clone() => Clone();

        public static IRichDataFieldsBuildOptions WithDefault(int maxDepth, bool includeSourcePath, bool allowCaching)
        {
            return new RichDataFieldsBuildOptions()
            {
                MaxDepth = maxDepth,
                IncludeSourcePath = includeSourcePath,
                AllowCaching = allowCaching
            };
        }

        public static IRichDataFieldsBuildOptions WithDefault(int maxDepth)
        {
            return new RichDataFieldsBuildOptions()
            {
                MaxDepth = maxDepth,
            };
        }
    }
}
