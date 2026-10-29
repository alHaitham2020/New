using ISoft.Utils.DataAnnotations;
using ISoft.Utils.Extentions;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using ISoft.Utils.Enumrations;
using System.Collections.Concurrent;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.RichReports.Data.DataAnnotations;

namespace ISoft.RichReports.Data.Fields
{
    /// <summary>
    /// Builds rich data field structures from object types using reflection with caching.
    /// Supports nested types, collections, and configurable depth/options.
    /// Thread-safe via ConcurrentDictionary and designed for performance.
    /// </summary>
    public class RichDataFieldsReflectionBuilder : IRichDataFieldsBuilder
    {
        #region Cache Structures

        /// <summary>
        /// Cached metadata for a type - immutable once created.
        /// Designed to be thread-safe for concurrent reads.
        /// </summary>
        private class CachedTypeMetadata
        {
            public Type TargetType { get; set; }

            public CachedPropertyMetadata[] Properties { get; set; } = Array.Empty<CachedPropertyMetadata>();

            // calculated every call to get a loclized display name
            public string GetDisplayName() =>
                DataAnnotationsHelper.GetLocalOrUnderlyingDisplayName(TargetType, propertyNameIfNull: true, inherit: true);
        }

        /// <summary>
        /// Cached metadata for a single property - immutable once created.
        /// Pre-computes reflection-heavy values for performance.
        /// </summary>
        private class CachedPropertyMetadata
        {
            public PropertyInfo Property { get; set; }
            public Type PropertyType { get; set; }
            public Type UnderlyingType { get; set; }
            public bool IsMutableReferenceType { get; set; }
            public bool IsDetailCollection { get; set; }
            public Type ChildTargetType { get; set; }
            public DataTypeKind DataTypeKind { get; set; }
            public RichFieldDataTypeCategory DataTypeCategory { get; set; }
            public RichDataFieldAttribute RichFieldAttribute { get; set; }
            public RichDataFieldFormatAttributeBase FormatAttribute { get; set; }
            public RichDataFieldImageAttribute ImageAttribute { get; set; }

            public string GetDisplayName() =>
                DataAnnotationsHelper.GetLocalOrUnderlyingDisplayName(Property, propertyNameIfNull: true, inherit: true);
        }

        #endregion

        #region Fields

        // Thread-safe reflection cache shared across all instances - reduces reflection overhead
        private static readonly ConcurrentDictionary<Type, CachedTypeMetadata> _metadataCache = new();

        // Default builder instance with default options
        private static readonly Lazy<RichDataFieldsReflectionBuilder> _default =
            new(() => new RichDataFieldsReflectionBuilder(new RichDataFieldsBuildOptions()));

        public static IRichDataFieldsBuilder Default => _default.Value;

        private readonly IRichDataFieldsBuildOptions _options;

        #endregion

        #region Constructors & Factory Methods

        public RichDataFieldsReflectionBuilder(IRichDataFieldsBuildOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public static RichDataFieldsReflectionBuilder WithOptions(IRichDataFieldsBuildOptions options) =>
            new(options);

        public static RichDataFieldsReflectionBuilder WithOptions(Action<IRichDataFieldsBuildOptions> configure)
        {
            var options = new RichDataFieldsBuildOptions();
            configure?.Invoke(options);
            return new RichDataFieldsReflectionBuilder(options);
        }

        #endregion

        #region IRichDataFieldsBuilder Implementation

        public RichDataFieldsReflectionBuilder WithCurrentOptions(Action<IRichDataFieldsBuildOptions> configure)
        {
            var options = _options.Clone();
            configure?.Invoke(options);
            return new RichDataFieldsReflectionBuilder(options);
        }

        public RichDataFieldGroup[] BuildFromObject<T>(params RichDataFieldGroup[] defaultGroups) =>
            BuildFromType(typeof(T), defaultGroups);

        public RichDataFieldGroup[] BuildFromObject(object source, params RichDataFieldGroup[] defaultGroups)
        {
            ArgumentGuardHelper.ArgumentNotNull(source);
            return BuildFromType(source.GetType(), defaultGroups);
        }

        public RichDataFieldGroup[] BuildFromType(Type sourceType, params RichDataFieldGroup[] defaultGroups)
        {
            ArgumentGuardHelper.ArgumentNotNull(sourceType);
            var actualType = sourceType.GetCollectionItemTypeOrUnderlyingType();
            return BuildFields(actualType, defaultGroups);
        }

        public void ClearCache() => _metadataCache.Clear();

        #endregion

        #region Core Building Logic

        /// <summary>
        /// Entry point for building field groups from a type.
        /// Recursively processes type hierarchy respecting depth and visited type limits.
        /// </summary>
        private RichDataFieldGroup[] BuildFields(Type type, params RichDataFieldGroup[] defaultGroups)
        {
            var result = new List<RichDataFieldGroup>();
            var rootPath = _options.IncludeSourcePath ? type.Name : string.Empty;

            ProcessType(
                type: type,
                result: result,
                visited: [],
                path: rootPath,
                groupProperty: null,
                depth: 0,
                isDetailCollection: false,
                isRoot: true,
                parentGroup: null);

            if (defaultGroups is { Length: > 0 })
                result.AddRange(defaultGroups);

            // Reverse to maintain parent-before-child ordering
            result.Reverse();
            return result.ToArray();
        }

        /// <summary>
        /// Recursively processes a type, building field groups and nested groups.
        /// Uses cached metadata to avoid repeated reflection.
        /// </summary>
        private void ProcessType(
            Type type,
            List<RichDataFieldGroup> result,
            HashSet<Type> visited,
            string path,
            PropertyInfo groupProperty,
            int depth,
            bool isDetailCollection,
            bool isRoot,
            RichDataFieldGroup parentGroup = null)
        {
            var targetType = type.GetUnderlyingTypeOrType();

            // Guard against circular references and depth limits
            if (depth > _options.MaxDepth || !visited.Add(targetType))
                return;

            try
            {
                // Get or create cached metadata - this is the performance win
                var typeMetadata = _metadataCache.GetOrAdd(targetType, CreateTypeCachedMetadata);

                var groupType = (groupProperty?.PropertyType ?? targetType)?.GetCollectionItemTypeOrType();
                var groupName = groupProperty?.Name ?? targetType.Name;

                var groupDisplayName = groupProperty != null
                    ? DataAnnotationsHelper.GetLocalOrUnderlyingDisplayName(groupProperty, propertyNameIfNull: true, inherit: true)
                    : typeMetadata.GetDisplayName();

                var group = new RichDataFieldGroup(
                    name: groupName,
                    displayName: groupDisplayName,
                    path: path,
                    dataType: groupType,
                    parentGroup: parentGroup);

                var fields = new List<RichDataField>();

                // Process each property using cached metadata
                foreach (var propMetadata in typeMetadata.Properties)
                {
                    // Apply runtime filters that can't be cached globally
                    if (_options.ExcludedTypes.Contains(propMetadata.UnderlyingType))
                        continue;

                    var propertyPath = isRoot
                        ? propMetadata.Property.Name
                        : RichFieldDataPathHelper.BuildPath(path, propMetadata.Property.Name);

                    // Handle nested reference types and collections recursively
                    if (propMetadata.IsMutableReferenceType && propMetadata.ChildTargetType != null)
                    {
                        ProcessType(
                            type: propMetadata.ChildTargetType,
                            result: result,
                            visited: visited,
                            path: propertyPath,
                            groupProperty: propMetadata.Property,
                            depth: depth + 1,
                            isDetailCollection: propMetadata.IsDetailCollection,
                            isRoot: false,
                            parentGroup: group);

                        continue;
                    }

                    // Create a leaf field from cached metadata
                    fields.Add(CreateFieldFromCachedMetadata(propMetadata, propertyPath, isDetailCollection, group));
                }

                // Only add groups that have fields
                if (fields.Count > 0)
                {
                    group.Fields.LoadFrom(fields);
                    result.Add(group);
                }
            }
            finally
            {
                // Always clean up visited set to allow other paths
                visited.Remove(targetType);
            }
        }

        #endregion

        #region Caching & Field Creation

        /// <summary>
        /// Creates cached metadata for a type by reflecting over its public instance properties.
        /// This is called only once per type and cached for reuse.
        /// </summary>
        private CachedTypeMetadata CreateTypeCachedMetadata(Type type)
        {
            var validProperties = new List<CachedPropertyMetadata>();

            foreach (var property in type.GetPublicInstanceProperties())
            {
                // Skip properties that can't be read or are marked ignored
                if (!ShouldProcessProperty(property))
                    continue;

                var propertyType = property.PropertyType;
                var underlyingType = propertyType.GetUnderlyingTypeOrType();
                var isMutableRef = propertyType.IsMutableReferenceType();
                var isDetailCollection = underlyingType.IsCollection();

                Type childTargetType = null;

                if (isMutableRef)
                    childTargetType = isDetailCollection ? underlyingType.GetCollectionItemType() : underlyingType;

                validProperties.Add(new CachedPropertyMetadata
                {
                    Property = property,
                    PropertyType = propertyType,
                    UnderlyingType = underlyingType,
                    IsMutableReferenceType = isMutableRef,
                    IsDetailCollection = isDetailCollection,
                    ChildTargetType = childTargetType,
                    RichFieldAttribute = property.FindFluentOrCustomAttribute<RichDataFieldAttribute>(),
                    FormatAttribute = property.FindFluentOrCustomAttribute<RichDataFieldFormatAttributeBase>(),
                    ImageAttribute = property.FindFluentOrCustomAttribute<RichDataFieldImageAttribute>(),
                    DataTypeKind = underlyingType.GetDataTypeKind(),
                    DataTypeCategory = underlyingType.GetFieldCategory()
                });
            }

            return new CachedTypeMetadata
            {
                TargetType = type,
                Properties = validProperties.ToArray()
            };
        }

        /// <summary>
        /// Creates a RichDataField from cached property metadata.
        /// Applies options for null values and formatting.
        /// </summary>
        private RichDataField CreateFieldFromCachedMetadata(
            CachedPropertyMetadata meta,
            string fieldPath,
            bool isDetailItemField,
            RichDataFieldGroup parentGroup)
        {
            return new RichDataField
            {
                Name = meta.Property.Name,
                DisplayName = meta.GetDisplayName(),
                Path = fieldPath,
                FieldProperty = fieldPath,
                DataMember = isDetailItemField ? parentGroup.Path : string.Empty,
                IsDetailField = isDetailItemField,
                DataTypeKind = meta.DataTypeKind,
                DataTypeCategory = (meta.ImageAttribute?.SchemeType ?? Office.Images.ImageUriSchemeType.None) != Office.Images.ImageUriSchemeType.None ? RichFieldDataTypeCategory.Image : meta.DataTypeCategory,
                DataType = meta.PropertyType,
                NullValue = GetPropertyDefaultNullValue(meta.Property, meta.RichFieldAttribute),
                FieldFormat = GetPropertyDefaultFormat(meta.UnderlyingType, meta.RichFieldAttribute, meta.FormatAttribute),
                AllowUseDefaultEmptyValue = meta.RichFieldAttribute?.AllowUseDefaultEmptyValue ?? true,
                ImageSchemeType = meta.ImageAttribute?.SchemeType ?? Office.Images.ImageUriSchemeType.None,
                ParentGroup = parentGroup,
                IsVisible = true,
            };
        }

        /// <summary>
        /// Determines if a property should be processed.
        /// Checks readability and exclusion attributes.
        /// </summary>
        private static bool ShouldProcessProperty(PropertyInfo property)
        {
            return property.CanRead && 
                   !property.HasFluentOrCustomAttribute<CommonIgnoreAttribute>() && 
                   !property.HasFluentOrCustomAttribute<RichDataFieldIgnoreAttribute>();
        }

        /// <summary>
        /// Determines the default null value for a property.
        /// Respects options, attribute overrides, and property defaults.
        /// </summary>
        private object GetPropertyDefaultNullValue(PropertyInfo property, RichDataFieldAttribute attribute)
        {
            if (!_options.AllowUseDefaultEmptyValue)
                return null;

            if (attribute?.NullValue is not null)
                return attribute.NullValue;

            if (attribute?.AllowUseDefaultEmptyValue == false)
                return null;

            return property.GetDefaultOrEmptyValue();
        }

        /// <summary>
        /// Determines the default format for a property.
        /// Uses attribute-specified format or falls back to default based on type.
        /// </summary>
        private RichFieldFormat? GetPropertyDefaultFormat(Type underlyingType, RichDataFieldAttribute attribute, RichDataFieldFormatAttributeBase formatAttribute)
        {
            if (!_options.AllowUseDefaultFormat)
                return null;

            var format = formatAttribute?.GetFormat();
            if (format != null)
            {
                return format;
            }

            if (attribute?.AllowUseDefaultFormat == false)
                return null;

            return RichFieldFormatHelper.GetDefaultFormat(underlyingType);
        }

        #endregion
    }
}
