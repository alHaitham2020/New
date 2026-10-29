using ISoft.RichReports.Core.Enums;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Core.Fields
{
    public static class RichCalculatedFieldTypeMapper
    {
        // ---------------------------------------------------------------------
        // Single source of truth: RichCalculatedFieldType -> .NET Type
        // Both ToClrType (public) and FromClrType (reverse) read from this,
        // so adding a new enum member only requires editing one place.
        // ---------------------------------------------------------------------
        private static readonly IReadOnlyDictionary<RichCalculatedFieldType, Type> ClrTypeMap =
            new Dictionary<RichCalculatedFieldType, Type>
            {
                [RichCalculatedFieldType.None] = typeof(object),
                [RichCalculatedFieldType.String] = typeof(string),
                [RichCalculatedFieldType.DateTime] = typeof(DateTime),
                [RichCalculatedFieldType.Date] = typeof(DateOnly),
                [RichCalculatedFieldType.Time] = typeof(TimeOnly),
                [RichCalculatedFieldType.Int] = typeof(int),
                [RichCalculatedFieldType.Float] = typeof(float),
                [RichCalculatedFieldType.Double] = typeof(double),
                [RichCalculatedFieldType.Decimal] = typeof(decimal),
                [RichCalculatedFieldType.Boolean] = typeof(bool),
                [RichCalculatedFieldType.Guid] = typeof(Guid),
            };

        // ---------------------------------------------------------------------
        // Public: RichCalculatedFieldType? -> .NET Type
        // Null / unknown values default to typeof(object) for the same reason.
        // ---------------------------------------------------------------------
        public static Type ToClrType(this RichCalculatedFieldType? fieldType)
        {
            if (fieldType is null)
                return typeof(object);

            return ClrTypeMap.TryGetValue(fieldType.Value, out var result)
                ? result
                : typeof(object);
        }

        // ---------------------------------------------------------------------
        // Public: .NET Type -> RichCalculatedFieldType
        // Unwraps Nullable<T> so typeof(int?) maps to Int, not None.
        // Unknown types return None (matches the original behavior).
        // ---------------------------------------------------------------------
        public static RichCalculatedFieldType FromClrType(Type type)
        {
            if (type is null)
                return RichCalculatedFieldType.None;

            type = Nullable.GetUnderlyingType(type) ?? type;

            // Reverse lookup against the same map used by ToClrType.
            foreach (var pair in ClrTypeMap)
            {
                if (pair.Value == type)
                    return pair.Key;
            }

            return RichCalculatedFieldType.None;
        }
    }
}
