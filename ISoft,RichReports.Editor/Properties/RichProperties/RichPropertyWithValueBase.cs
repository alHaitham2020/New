using ISoft.Office.Images;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.Helpers;
using System;
using System.Runtime.CompilerServices;

namespace ISoft.RichReports.Editor.RichProperties
{
    public interface IRichPropertyWithValue : IRichProperty
    {
        object Value { get; }

        ImageUriSchemeType ImageUriSchemeType { get; }

        /// <summary>
        /// Returns a propertyValue or defaultValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetPropertyValue<T>();

        string GetStringValue();

        object GetValueOrDefault();
    }

    /// <summary>
    /// Represents a custom document property with type-safe value access
    /// </summary>
    public abstract class RichPropertyWithValueBase : RichPropertyBase, IRichPropertyWithValue
    {
        #region Proprties
        private object _value;
        public object Value
        {
            get => _value;
            set => _value = value;
        }

        public ImageUriSchemeType ImageUriSchemeType
        {
            get
            {
                if (PropertyType != null && PropertyType.IsDerivedFrom(typeof(ImageUri)))
                {
                    return GetPropertyValue(ImageUri.Empty).Scheme.Type;
                }

                return ImageUriSchemeType.None;
            }
        }

        #endregion

        public RichPropertyWithValueBase() { }

        public RichPropertyWithValueBase(
            string name,
            string displayName,
            Type propertyType,
            object value,
            object defaultValue = null,
            RichFieldFormat? format = null,
            bool? visible = null)
            : base(name, displayName, defaultValue, propertyType, format, visible)
        {
            _value = value;
        }

        #region Actions
        /// <summary>
        /// Gets the property value as a string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetStringValue() => GetPropertyValue(GetDefaultValue<string>());

        public object GetValueOrDefault() => _value ?? DefaultValue;

        /// <summary>
        /// Gets the property value with type safety
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T GetPropertyValue<T>() => GetPropertyValue(GetDefaultValue<T>());

        /// <summary>
        /// Gets the property value with a custom default
        /// </summary>
        public T GetPropertyValue<T>(T defaultValue)
        {
            if (_value is null)
                return defaultValue;

            try
            {
                return ConvertValue(_value, defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected virtual T GetDefaultValue<T>()
        {
            if (_value is null)
                return default;

            try
            {
                return ConvertValue<T>(DefaultValue, default);
            }
            catch
            {
                return default;
            }
        }

        protected virtual T ConvertValue<T>(object value, T defaultValue)
        {
            return value.TryConvertValue(PropertyType, defaultValue);
        }

        /// <summary>
        /// Tries to get the property value with type safety
        /// </summary>
        public bool TryGetPropertyValue<T>(out T value)
        {
            return TryGetPropertyValue(out value, GetDefaultValue<T>());
        }

        /// <summary>
        /// Tries to get the property value with a custom default
        /// </summary>
        public bool TryGetPropertyValue<T>(out T value, T defaultValue)
        {
            try
            {
                value = GetPropertyValue(defaultValue);
                return true;
            }
            catch
            {
                value = defaultValue;
                return false;
            }
        }

        /// <summary>
        /// Checks if the property value can be converted to the specified type
        /// </summary>
        public bool CanConvertTo<T>()
        {
            if (_value is null)
                return false;

            try
            {
                ConvertValue<T>(_value, default);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override string ToString() => GetStringValue();

        #endregion
    }
}
