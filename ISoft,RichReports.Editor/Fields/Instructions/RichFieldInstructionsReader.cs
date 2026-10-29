using ISoft.RichReports.Editor.Fields.Argumentes;
using ISoft.RichReports.Editor.Fields.Codes;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Options;
using ISoft.RichReports.Editor.Fields.Switches;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace ISoft.RichReports.Editor.Fields.Instructions
{
    public interface IRichFieldInstructionsReader
    {
        object Read(IRichFieldCode fieldCode);

        T ReadeAs<T>(IRichFieldCode fieldCode) where T : class, IRichFieldOptions;

        void Populate(IRichFieldCode fieldCode, object options);

        void Populate(IRichFieldCode fieldCode, object options, InstructionsBuilderTargets targets);
    }

    public class RichFieldInstructionsReader : IRichFieldInstructionsReader
    {
        #region Nested Components

        /// <summary>
        /// Thread-safe cache of per-type property metadata to avoid repeated reflection.
        /// </summary>
        private static class PropertyInfoCache
        {
            private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyMetadata>> _cache = new();

            public static IReadOnlyList<PropertyMetadata> GetProperties(Type type) =>
                _cache.GetOrAdd(type, static t =>
                    t.GetPublicInstanceValueTypeProperties()
                     .Select(p => new PropertyMetadata(p))
                     .Where(m => m.HasBehaviorAttribute)
                     .ToList());
        }

        /// <summary>
        /// Precomputed metadata for a property, identifying its singular active attribute
        /// based on precedence (Argument > Switch > FormatSwitch) and caching the
        /// string-to-type converter so reads don't pay descriptor lookup costs.
        /// </summary>
        private sealed class PropertyMetadata
        {
            public PropertyInfo Property { get; }
            public RichFieldArgumentAttribute ArgumentAttribute { get; }
            public RichFieldSwitchAttribute SwitchAttribute { get; }
            public RichFieldFormatSwitchAttribute FormatSwitchAttribute { get; }
            public InstructionsBuilderTargets Target { get; }

            /// <summary>
            /// Cached TypeConverter for the property's (unwrapped) type, or null when
            /// the property is a string / enum / bool / RichFieldFormat and doesn't need one.
            /// </summary>
            public TypeConverter StringConverter { get; }

            public bool HasBehaviorAttribute => Target != 0;

            public PropertyMetadata(PropertyInfo property)
            {
                Property = property;
                var attributes = DataAnnotationsHelper.GetAllAttributes(property);

                // Same priority hierarchy as the builder: Argument > Switch > FormatSwitch.
                if ((ArgumentAttribute = attributes.OfType<RichFieldArgumentAttribute>().FirstOrDefault()) != null)
                {
                    Target = InstructionsBuilderTargets.Arguments;
                }
                else if ((SwitchAttribute = attributes.OfType<RichFieldSwitchAttribute>().FirstOrDefault()) != null)
                {
                    Target = InstructionsBuilderTargets.Switches;
                }
                else if ((FormatSwitchAttribute = attributes.OfType<RichFieldFormatSwitchAttribute>().FirstOrDefault()) != null)
                {
                    Target = InstructionsBuilderTargets.Switches;
                }

                // Cache the string converter once per property.
                var underlying = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                if (underlying != typeof(string) && !underlying.IsEnum && underlying != typeof(bool))
                {
                    var converter = TypeDescriptor.GetConverter(underlying);
                    if (converter.CanConvertFrom(typeof(string)))
                        StringConverter = converter;
                }
            }
        }

        #endregion

        #region Fields

        private readonly IRichEditFieldOptionsFactory _factory;

        #endregion

        public RichFieldInstructionsReader(IRichEditFieldOptionsFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        #region PUBLIC API

        /// <summary>
        /// Creates a new options instance for <paramref name="fieldCode"/>'s field type
        /// and populates it from the field code's arguments and switches.
        /// Returns <c>null</c> when <paramref name="fieldCode"/> is <c>null</c>.
        /// </summary>
        public object Read(IRichFieldCode fieldCode)
        {
            if (fieldCode == null) return null;

            var options = _factory.Create(fieldCode.FieldType);
            ReadInto(options, fieldCode, InstructionsBuilderTargets.All);
            return options;
        }

        /// <summary>
        /// Strongly-typed convenience overload of <see cref="Read(IRichFieldCode)"/>.
        /// </summary>
        public T ReadeAs<T>(IRichFieldCode fieldCode) where T : class, IRichFieldOptions
            => (T)Read(fieldCode);

        /// <summary>
        /// Populates an existing options instance from the field code (all targets).
        /// </summary>
        public void Populate(IRichFieldCode fieldCode, object options)
            => ReadInto(options, fieldCode, InstructionsBuilderTargets.All);

        /// <summary>
        /// Populates an existing options instance from the field code for the given targets.
        /// Useful when only arguments or only switches should be hydrated.
        /// </summary>
        public void Populate(IRichFieldCode fieldCode, object options, InstructionsBuilderTargets targets)
            => ReadInto(options, fieldCode, targets);

        #endregion

        #region Central Processing Engine

        /// <summary>
        /// Core engine: iterates cached property metadata, filters by target, applies
        /// the options contract, then hydrates the property from the field code.
        /// Mirror of <c>RichFieldInstructionsBuilder.ProcessProperties</c>.
        /// </summary>
        private void ReadInto(object options, IRichFieldCode fieldCode, InstructionsBuilderTargets allowedTargets)
        {
            if (fieldCode == null) throw new ArgumentNullException(nameof(fieldCode));
            ValidateOptions(options);

            var properties = PropertyInfoCache.GetProperties(options.GetType());
            var optionsContract = options as IRichFieldOptions;

            foreach (var metadata in properties)
            {
                // 1. Target filter.
                if ((metadata.Target & allowedTargets) == 0)
                    continue;

                // 2. Options contract filter.
                if (!ShouldApplyProperty(metadata, optionsContract))
                    continue;

                // 3. Route to the correct reader.
                try
                {
                    if (metadata.ArgumentAttribute != null)
                        ReadArgument(options, metadata, metadata.ArgumentAttribute, fieldCode.Arguments);
                    else if (metadata.SwitchAttribute != null)
                        ReadSwitch(options, metadata, metadata.SwitchAttribute, fieldCode.Switches);
                    else if (metadata.FormatSwitchAttribute != null)
                        ReadFormatSwitch(options, metadata, fieldCode.Switches);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Error reading instructions for property {metadata.Property.Name}", ex);
                }
            }
        }

        #endregion

        #region Actions

        private static void ValidateOptions(object options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (options is not IRichFieldOptions)
                throw new InvalidOperationException($"field options must be of type {nameof(IRichFieldOptions)}");
        }

        /// <summary>
        /// Same contract filtering as the builder, so asymmetric registrations cannot
        /// cause a property to be written on one side and silently dropped on the other.
        /// </summary>
        private static bool ShouldApplyProperty(PropertyMetadata metadata, IRichFieldOptions contract)
        {
            if (contract == null) return true;

            if (metadata.ArgumentAttribute != null && !contract.CanApplyArgumentValue(metadata.Property.Name))
                return false;

            if ((metadata.SwitchAttribute != null || metadata.FormatSwitchAttribute != null)
                && !contract.CanApplySwitchValue(metadata.Property.Name))
                return false;

            return true;
        }

        private static void ReadArgument(
            object options,
            PropertyMetadata metadata,
            RichFieldArgumentAttribute attribute,
            IRichFieldArgumentCollection args)
        {
            if (args == null) return;
            if (!args.TryGetValue<string>(attribute.Index, out var rawValue)) return;
            if (string.IsNullOrEmpty(rawValue)) return;

            SetPropertyValue(options, metadata, rawValue);
        }

        /// <summary>
        /// Reads a switch into the property. Boolean flags are presence-based
        /// (present → true, absent → false / null); everything else requires a value.
        /// </summary>
        private static void ReadSwitch(
            object options,
            PropertyMetadata metadata,
            RichFieldSwitchAttribute attribute,
            IRichFieldSwitchCollection switches)
        {
            if (switches == null) return;

            var propertyType = metadata.Property.PropertyType;

            if (propertyType == typeof(bool))
            {
                metadata.Property.SetValue(options, switches.ContainsKey(attribute.Key));
                return;
            }

            if (propertyType == typeof(bool?))
            {
                // Absent nullable flag stays null so round-tripping doesn't
                // clobber an "unspecified" state with "false".
                metadata.Property.SetValue(
                    options,
                    switches.ContainsKey(attribute.Key) ? (object)true : null);
                return;
            }

            if (!switches.TryGetItem(attribute.Key, out var fieldSwitch) || fieldSwitch == null)
                return;

            var rawValue = fieldSwitch.SwitchValue;
            if (string.IsNullOrEmpty(rawValue)) return;

            SetPropertyValue(options, metadata, rawValue);
        }

        private static void ReadFormatSwitch(
            object options,
            PropertyMetadata metadata,
            IRichFieldSwitchCollection switches)
        {
            if (switches == null) return;
            if (!switches.TryGetFormat(out var format)) return;
            if (RichFieldFormat.IsNullOrEmpty(format)) return;

            metadata.Property.SetValue(options, format);
        }

        /// <summary>
        /// Converts a raw string into the property's type and assigns it. Handles
        /// string / enum directly; anything else uses the cached <see cref="TypeConverter"/>
        /// (covers Guid, DateTime, TimeSpan, primitives, and custom converters).
        /// </summary>
        private static void SetPropertyValue(object options, PropertyMetadata metadata, string rawValue)
        {
            var propertyType = metadata.Property.PropertyType;
            var underlying = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            object converted;

            if (underlying == typeof(string))
            {
                converted = rawValue;
            }
            else if (underlying.IsEnum)
            {
                try
                {
                    converted = Enum.Parse(underlying, rawValue, ignoreCase: true);
                }
                catch (ArgumentException ex)
                {
                    throw new InvalidOperationException(
                        $"'{rawValue}' is not a valid value for enum '{underlying.Name}' " +
                        $"on property '{metadata.Property.Name}'.", ex);
                }
            }
            else if (metadata.StringConverter != null)
            {
                try
                {
                    converted = metadata.StringConverter.ConvertFromString(null, CultureInfo.InvariantCulture, rawValue);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        $"Cannot convert '{rawValue}' to '{underlying.Name}' " +
                        $"for property '{metadata.Property.Name}'.", ex);
                }
            }
            else
            {
                throw new InvalidOperationException(
                    $"No string converter available for property " +
                    $"'{metadata.Property.Name}' of type '{underlying.Name}'.");
            }

            metadata.Property.SetValue(options, converted);
        }

        #endregion
    }
}
