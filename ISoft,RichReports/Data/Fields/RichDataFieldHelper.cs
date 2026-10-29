using ISoft.Office.Images;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public static class RichDataFieldHelper
    {
        private static readonly Dictionary<Type, RichFieldDataTypeCategory> _typeMappings = new()
        {
            [typeof(int)] = RichFieldDataTypeCategory.Numeric,
            [typeof(long)] = RichFieldDataTypeCategory.Numeric,
            [typeof(short)] = RichFieldDataTypeCategory.Numeric,
            [typeof(decimal)] = RichFieldDataTypeCategory.Numeric,
            [typeof(double)] = RichFieldDataTypeCategory.Numeric,
            [typeof(float)] = RichFieldDataTypeCategory.Numeric,

            [typeof(string)] = RichFieldDataTypeCategory.String,
            [typeof(Enum)] = RichFieldDataTypeCategory.String,

            [typeof(bool)] = RichFieldDataTypeCategory.Boolean,

            [typeof(byte[])] = RichFieldDataTypeCategory.Image,

            [typeof(DateTime)] = RichFieldDataTypeCategory.DateTime,
            [typeof(DateOnly)] = RichFieldDataTypeCategory.DateTime,
            [typeof(TimeOnly)] = RichFieldDataTypeCategory.DateTime
        };

        // Fixed typo: RegisterCatrgoey -> RegisterCategory
        internal static void RegisterCategory(Type type, RichFieldDataTypeCategory category)
        {
            _typeMappings[type] = category;
        }

        internal static RichFieldDataTypeCategory GetFieldCategory(this Type dataType)
            => _typeMappings.TryGetValue(dataType, out var value) ? value : RichFieldDataTypeCategory.Other;

        internal static RichFieldDataTypeCategory GetFieldCategory(this Type dataType, ImageUriSchemeType schemeType)
        {
            return schemeType != ImageUriSchemeType.None ? RichFieldDataTypeCategory.Image : dataType.GetFieldCategory();
        }
    }
}
