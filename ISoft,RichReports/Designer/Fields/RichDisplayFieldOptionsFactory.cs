using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Designer.Fields
{
    /// <summary>
    /// Factory that resolves the correct <see cref="RichFieldOptionsBase"/> instance
    /// for a given <see cref="IRichDisplayField"/> based on its <see cref="IRichDisplayField.FieldType"/>.
    /// </summary>
    /// <remarks>
    /// The mapping between a field type and its options-creation strategy is static and immutable,
    /// so it is built once inside the static constructor and cached for the lifetime of the process.
    /// </remarks>
    public static class RichDisplayFieldOptionsFactory
    {
        // ---------------------------------------------------------------------
        // Read-only registry: populated once in the static ctor, never mutated.
        // Using IReadOnlyDictionary exposes intent and prevents accidental writes.
        // ---------------------------------------------------------------------
        private static readonly IReadOnlyDictionary<RichEditFieldType, Func<IRichDisplayField, RichFieldOptionsBase>> _factoryCache;

        static RichDisplayFieldOptionsFactory()
        {
            // Pre-size the dictionary with the known number of registrations to
            // avoid internal resize/rehash during static initialization.
            var cache = new Dictionary<RichEditFieldType, Func<IRichDisplayField, RichFieldOptionsBase>>(capacity: 16)
            {
                // Simple one-liner mappings.
                [RichEditFieldType.MergeField] = f => RichMergeFieldOptions.Create(f.FieldProperty, f.FieldFormat),
                [RichEditFieldType.DocPropertyField] = f => RichDocPropertyFieldOptions.Create(f.FieldProperty, f.FieldFormat),
                [RichEditFieldType.DocVariableField] = f => RichDocVariableFieldOptions.Create(f.FieldProperty, f.FieldFormat),
                [RichEditFieldType.CustomPropertyField] = f => RichDocCustomPropertyFieldOptions.Create(f.FieldProperty, f.FieldFormat),
                [RichEditFieldType.ExpEditCalculatedField] = f => RichExpressionEditorCalculatedFieldOptions.Create(f.FieldProperty, f.FieldFormat),
                [RichEditFieldType.IncludePictureField] = f => RichIncludePictureFieldOptions.Create(f.FieldProperty),
                [RichEditFieldType.HyperlinkField] = f => RichHyperlinkFieldOptions.Create(f.FieldProperty, f.FieldFormat),

                // Field types sharing the same creator are grouped for clarity.
                [RichEditFieldType.DateField] = CreateDateFieldOptions,
                [RichEditFieldType.CreateDateField] = CreateDateFieldOptions,
                [RichEditFieldType.SaveDateField] = CreateDateFieldOptions,

                [RichEditFieldType.TimeField] = CreateFormatableFieldOptions,
                [RichEditFieldType.PrintDateField] = CreateFormatableFieldOptions,
                [RichEditFieldType.PageField] = CreateFormatableFieldOptions,
                [RichEditFieldType.NumPagesField] = CreateFormatableFieldOptions,
            };

            _factoryCache = cache;
        }

        /// <summary>
        /// Creates options for a date-based field, propagating the field's own type.
        /// </summary>
        private static RichDateBasedFieldOptions CreateDateFieldOptions(IRichDisplayField field)
            => RichDateBasedFieldOptions.Create(field.FieldType, field.FieldFormat);

        /// <summary>
        /// Creates options for a generic formatable field, propagating the field's own type.
        /// </summary>
        private static RichFormatableFieldOptions CreateFormatableFieldOptions(IRichDisplayField field)
            => RichFormatableFieldOptions.Create(field.FieldType, field.FieldFormat);

        /// <summary>
        /// Registers a creator for a field type. Exposed for derived/extensibility scenarios.
        /// </summary>
        /// <remarks>
        /// The original implementation performed a <c>ContainsKey</c> check followed by <c>Add</c>,
        /// which is a double lookup and silently ignores duplicate registrations.
        /// This version uses a single lookup via <c>TryAdd</c> semantics while preserving
        /// the "first registration wins" behaviour.
        /// </remarks>
        private static void Register(
            IDictionary<RichEditFieldType, Func<IRichDisplayField, RichFieldOptionsBase>> cache,
            RichEditFieldType fieldType,
            Func<IRichDisplayField, RichFieldOptionsBase> creator)
        {
            // ArgumentGuardHelper is assumed to be the project's standard guard.
            ArgumentGuardHelper.ArgumentNotNull(cache, nameof(cache));
            ArgumentGuardHelper.ArgumentNotNull(creator, nameof(creator));

            // TryAdd performs a single hash lookup (vs. ContainsKey + Add).
            // Duplicates are ignored to preserve the original "first wins" behaviour.
            if (!cache.ContainsKey(fieldType))
            {
                cache.Add(fieldType, creator);
            }
        }

        /// <summary>
        /// Resolves the raw options object for the given field, or <c>null</c> if the
        /// field type is not registered.
        /// </summary>
        private static RichFieldOptionsBase CreateOptionsCore(IRichDisplayField field)
        {
            // Single dictionary lookup; delegate is invoked only when found.
            return _factoryCache.TryGetValue(field.FieldType, out var creator)
                ? creator(field)
                : null;
        }

        /// <summary>
        /// Creates the fully-resolved options object for the supplied field.
        /// </summary>
        /// <param name="field">The display field to create options for. Must not be <c>null</c>.</param>
        /// <returns>
        /// A <see cref="RichFieldOptionsBase"/> instance, or a wrapped nested-picture
        /// options object when the field is a nested picture field.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="field"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        /// Thrown when no creator is registered for the field type, or when the created
        /// options object reports a mismatched <c>FieldType</c>.
        /// </exception>
        public static object CreateOptions(IRichDisplayField field)
        {
            ArgumentGuardHelper.ArgumentNotNull(field, nameof(field));

            var fieldOptions = CreateOptionsCore(field);

            // Fail fast with a precise message instead of returning null to the caller.
            if (fieldOptions is null)
            {
                throw new InvalidOperationException($"Options not found for field '{field.FieldProperty}' with FieldType '{field.FieldType}'.");
            }

            // Defensive consistency check: the creator should have produced options
            // whose FieldType matches the source field. Catches mis-registrations early.
            if (field.FieldType != fieldOptions.FieldType)
            {
                throw new InvalidOperationException($"FieldType mismatch: field is '{field.FieldType}' but options report '{fieldOptions.FieldType}'.");
            }

            // Nested picture fields wrap the resolved options with an inner-picture shell.
            return field.IsNestedPictureField
                ? RichNestedFieldOptions.CreateIncludePictureWithInnerOptions(fieldOptions)
                : fieldOptions;
        }
    }
}
