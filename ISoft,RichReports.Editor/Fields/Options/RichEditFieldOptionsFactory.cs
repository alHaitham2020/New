using ISoft.RichReports.Editor.Fields.Core;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public interface IRichEditFieldOptionsFactory
    {
        object Create(RichEditFieldType fieldType);

        T CreateAs<T>(RichEditFieldType fieldType);
    }

    public sealed class RichEditFieldOptionsFactory : IRichEditFieldOptionsFactory
    {
        // Immutable lookup: field type -> concrete options type.
        // Built once in the static constructor; never mutated afterwards.
        private static readonly IReadOnlyDictionary<RichEditFieldType, Type> _optionsTypeByFieldType =
            new Dictionary<RichEditFieldType, Type>
            {
                // Dedicated option types.
                [RichEditFieldType.MergeField] = typeof(RichMergeFieldOptions),
                [RichEditFieldType.DocPropertyField] = typeof(RichDocPropertyFieldOptions),
                [RichEditFieldType.DocVariableField] = typeof(RichDocVariableFieldOptions),
                [RichEditFieldType.CustomPropertyField] = typeof(RichDocCustomPropertyFieldOptions),
                [RichEditFieldType.ExpEditCalculatedField] = typeof(RichExpressionEditorCalculatedFieldOptions),
                [RichEditFieldType.IncludePictureField] = typeof(RichIncludePictureFieldOptions),
                [RichEditFieldType.HyperlinkField] = typeof(RichHyperlinkFieldOptions),

                // Field types that share RichMergeFieldOptions.
                [RichEditFieldType.DateField] = typeof(RichMergeFieldOptions),
                [RichEditFieldType.CreateDateField] = typeof(RichMergeFieldOptions),
                [RichEditFieldType.SaveDateField] = typeof(RichMergeFieldOptions),
                [RichEditFieldType.TimeField] = typeof(RichMergeFieldOptions),
                [RichEditFieldType.PrintDateField] = typeof(RichMergeFieldOptions),
                [RichEditFieldType.PageField] = typeof(RichMergeFieldOptions),
                [RichEditFieldType.NumPagesField] = typeof(RichMergeFieldOptions),
            };

        public object Create(RichEditFieldType fieldType) 
            => CreateAs<object>(fieldType);

        public T CreateAs<T>(RichEditFieldType fieldType)
        {
            // Resolve & construct.
            var options = CreateCore(fieldType);

            // Validate before casting so misconfiguration surfaces with a clear message.
            Validate(fieldType, options);

            return (T)options;
        }

        /// <summary>
        /// Instantiates the registered options type for <paramref name="fieldType"/>.
        /// Throws if the field type has no registration.
        /// </summary>
        private static object CreateCore(RichEditFieldType fieldType)
        {
            if (!_optionsTypeByFieldType.TryGetValue(fieldType, out var optionsType))
            {
                throw new InvalidOperationException($"No options type registered for field type '{fieldType}'.");
            }

            return Activator.CreateInstance(optionsType);
        }

        private static void Validate(RichEditFieldType fieldType, object options)
        {
            if (options is not IRichFieldOptions fieldOptions)
            {
                throw new InvalidOperationException($"Options for '{fieldType}' must implement {nameof(IRichFieldOptions)}.");
            }

            // Guard against mis-registration: the created options must report the same field type.
            if (fieldOptions.FieldType != fieldType)
            {
                throw new InvalidOperationException($"Field type mismatch: requested '{fieldType}' but options report '{fieldOptions.FieldType}'.");
            }
        }
    }
}
