using ISoft.RichReports.Editor.Fields.Argumentes;
using ISoft.RichReports.Editor.Fields.Codes;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Editor.Fields.Options;
using ISoft.RichReports.Editor.Fields.Switches;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ISoft.RichReports.Editor.Fields.Instructions
{
    /// <summary>
    /// Builds reflection-based instructions for rich field codes using cached property metadata.
    /// Processes properties decorated with attributes in priority order: Argument > Switch > FormatSwitch.
    /// </summary>
    public class RichFieldInstructionsBuilder : IRichFieldInstructionsBuilder
    {
        #region Nested Components

        /// <summary>
        /// Thread-safe cache for property metadata per type to avoid repeated reflection.
        /// </summary>
        private static class PropertyInfoCache
        {
            private static readonly ConcurrentDictionary<Type, IReadOnlyList<PropertyMetadata>> _cache = new();

            public static IReadOnlyList<PropertyMetadata> GetProperties(Type type)
            {
                return _cache.GetOrAdd(type, t =>
                        t.GetPublicInstanceValueTypeProperties()
                         .Select(p => new PropertyMetadata(p))
                         .Where(m => m.HasBehaviorAttribute)
                         .ToList()
                );
            }
        }

        /// <summary>
        /// Precomputed metadata for a property, identifying its singular active attribute based on precedence.
        /// </summary>
        private class PropertyMetadata
        {
            public PropertyInfo Property { get; }

            public RichFieldArgumentAttribute ArgumentAttribute { get; }

            public RichFieldSwitchAttribute SwitchAttribute { get; }

            public RichFieldFormatSwitchAttribute FormatSwitchAttribute { get; }

            public InstructionsBuilderTargets Target { get; }

            public bool HasBehaviorAttribute => Target != 0;

            public PropertyMetadata(PropertyInfo property)
            {
                Property = property;
                var attributes = DataAnnotationsHelper.GetAllAttributes(property);

                // Establish priority hierarchy: Argument > Switch > FormatSwitch
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
            }
        }
        #endregion

        #region Fields
        private readonly IRichFieldSwitchBuilder _switchBuilder;
        #endregion

        public RichFieldInstructionsBuilder(IRichFieldSwitchBuilder switchBuilder)
        {
            _switchBuilder = switchBuilder ?? throw new ArgumentNullException(nameof(switchBuilder));
        }

        #region PUBLIC API

        /// <summary>
        /// Builds field switches by reflecting over the options object.
        /// Processes properties with RichFieldSwitchAttribute or RichFieldFormatSwitchAttribute.
        /// </summary>
        public string GetFieldSwitchesBuildString(object options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            RichFieldSwitchCollection switches = new();

            BuildFieldSwitches(switches, options);

            return switches.GetBuildString();
        }

        /// <summary>
        /// Builds field arguments by reflecting over the options object.
        /// Only processes properties with RichFieldArgumentAttribute.
        /// </summary>
        public string GetFieldArgumentsBuildString(object options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            RichFieldArgumentCollection arguments = new();

            BuildFieldArguments(arguments, options);

            return arguments.GetBuildString();
        }

        /// <summary>
        /// Builds field arguments by reflecting over the options object.
        /// Only processes properties with RichFieldArgumentAttribute.
        /// </summary>
        public void BuildFieldArguments(IRichFieldArgumentCollection arguments, object options)
        {
            if (arguments == null) throw new ArgumentNullException(nameof(arguments));
            ProcessProperties(options, InstructionsBuilderTargets.Arguments, args: arguments);
        }

        /// <summary>
        /// Builds field switches by reflecting over the options object.
        /// Processes properties with RichFieldSwitchAttribute or RichFieldFormatSwitchAttribute.
        /// </summary>
        public void BuildFieldSwitches(IRichFieldSwitchCollection switches, object options)
        {
            if (switches == null) throw new ArgumentNullException(nameof(switches));
            ProcessProperties(options, InstructionsBuilderTargets.Switches, sws: switches);
        }

        /// <summary>
        /// Instantiates field codes and routes assignments based on runtime types.
        /// </summary>
        //public IRichFieldCode CreateAndBuildInstructions(object options)
        //{
        //    if (options == null) throw new ArgumentNullException(nameof(options));

        //    if (options is not IRichFieldOptions fieldOptions)
        //        throw new InvalidOperationException($"field options must be of type {nameof(IRichFieldOptions)}");

        //    var fieldCode = new RichFieldCode(fieldOptions.FieldType);

        //    BuildInstructions(fieldCode, options);

        //    return fieldCode;
        //}

        /// <summary>
        /// Builds field code instructions by reflecting over the options object with all targets;
        /// </summary>
        public void BuildInstructions(IRichFieldCode fieldCode, object options)
        {
            BuildInstructions(fieldCode, options, InstructionsBuilderTargets.All);
        }

        /// <summary>
        /// Builds field code instructions by reflecting over the options object.
        /// </summary>
        public void BuildInstructions(IRichFieldCode fieldCode, object options, InstructionsBuilderTargets targets)
        {
            if (fieldCode == null) throw new ArgumentNullException(nameof(fieldCode));

            ProcessProperties(options, targets, fieldCode.Arguments, fieldCode.Switches);
        }

        #endregion

        #region Central Processing Engine

        /// <summary>
        /// Core engine handling iterative parsing, contract routing, value cleaning, and unified execution loops.
        /// </summary>
        private void ProcessProperties(
            object options,
            InstructionsBuilderTargets allowedTargets,
            IRichFieldArgumentCollection args = null,
            IRichFieldSwitchCollection sws = null)
        {
            ValidateOptions(options);

            var properties = PropertyInfoCache.GetProperties(options.GetType());

            var optionsContract = options as IRichFieldOptions;

            foreach (var metadata in properties)
            {
                // 1. Check if the target is requested in the current execution mode
                if ((metadata.Target & allowedTargets) == 0)
                    continue;

                // Apply contract-based filtering if available
                // 2. Validate against explicit options conditions contracts
                if (!ShouldApplyProperty(metadata, optionsContract))
                    continue;

                var value = metadata.Property.GetValue(options);

                // Skip null or default/empty values
                // 3. Extract and filter value expressions
                if (IsEmptyOrDefault(value, metadata))
                    continue;

                // 4. Safely apply updates via determined targets
                try
                {
                    // Priority: Argument > Switch > FormatSwitch
                    if (metadata.ArgumentAttribute != null && args != null)
                        ProcessArgument(value, metadata.ArgumentAttribute, args);

                    else if (metadata.SwitchAttribute != null && sws != null)
                        ProcessSwitch(value, metadata.SwitchAttribute, sws);

                    else if (metadata.FormatSwitchAttribute != null && sws != null)
                        ProcessFormatSwitch(value, sws);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Error building instructions for property {metadata.Property.Name}", ex);
                }
            }
        }

        /// <summary>
        /// Safely prepares custom switches mapped from non-boolean variables or valid booleans.
        /// </summary>
        private IRichFieldSwitch CreateSwitch(string key, object value)
        {
            if (value is bool boolValue)
            {
                return boolValue ? _switchBuilder.CreateFieldSwitch(key) : null;
            }

            var stringValue = value.ConvertValueToString();
            return string.IsNullOrEmpty(stringValue) ? null : _switchBuilder.CreateFieldSwitch(key, stringValue);
        }

        #endregion

        #region Actions
        private void ValidateOptions(object options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options is not IRichFieldOptions)
                throw new InvalidOperationException($"field options must be of type {nameof(IRichFieldOptions)}");
        }

        private bool ShouldApplyProperty(PropertyMetadata metadata, IRichFieldOptions optionsContract)
        {
            // If no contract, all properties are allowed
            if (optionsContract == null)
                return true;

            // Check argument contract
            if (metadata.ArgumentAttribute != null && !optionsContract.CanApplyArgumentValue(metadata.Property.Name))
                return false;

            // Check switch contract (covers both switch and format-switch)
            if ((metadata.SwitchAttribute != null || metadata.FormatSwitchAttribute != null) && !optionsContract.CanApplySwitchValue(metadata.Property.Name))
                return false;

            return true;
        }

        private bool ShouldApplyArgument(PropertyMetadata metadata, IRichFieldOptions optionsContract)
        {
            if (optionsContract == null)
                return false;

            return metadata.ArgumentAttribute != null && optionsContract.CanApplyArgumentValue(metadata.Property.Name);
        }

        private bool ShouldApplySwitch(PropertyMetadata metadata, IRichFieldOptions optionsContract)
        {
            if (optionsContract == null)
                return false;

            return metadata.SwitchAttribute != null && optionsContract.CanApplySwitchValue(metadata.Property.Name);
        }

        private bool ShouldApplyFormatSwitch(PropertyMetadata metadata, IRichFieldOptions optionsContract)
        {
            if (optionsContract == null)
                return false;

            return metadata.FormatSwitchAttribute != null;
        }

        private bool IsEmptyOrDefault(object value, PropertyMetadata metadata)
        {
            return value == null || value.IsDefaultOrEmptyPropertyValue(property: metadata.Property, nullValue: null, allowEmptyString: false);
        }

        private void ProcessArgument(object value, RichFieldArgumentAttribute attribute, IRichFieldArgumentCollection args)
        {
            var stringValue = value.ConvertValueToString();
            if (!string.IsNullOrEmpty(stringValue))
            {
                args.AddValueAtIndex(attribute.Index, stringValue);
            }
        }

        /// <summary>
        /// Processes a property with a Switch attribute.
        /// </summary>
        private void ProcessSwitch(object value, RichFieldSwitchAttribute attribute, IRichFieldSwitchCollection switches)
        {
            var fieldSwitch = CreateSwitch(attribute.Key, value);
            if (fieldSwitch != null)
            {
                switches.AddOrUpdate(fieldSwitch);
            }
        }

        private void ProcessFormatSwitch(object value, IRichFieldSwitchCollection switches)
        {
            if (value is RichFieldFormat format && !RichFieldFormat.IsNullOrEmpty(format))
            {
                switches.AddOrUpdate(_switchBuilder.CreateFormatSwitch(format));
            }
        }

        #endregion
    }
}
