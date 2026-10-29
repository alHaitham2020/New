using ISoft.Utils.Helpers;

namespace ISoft.RichReports.Editor.RichProperties
{
    public static class RichPropertiesExtensions
    {
        public static T GetPropertyValue<TProperty, T>(this IRichValueProperties<TProperty> properties, string name) where TProperty : class, IRichPropertyWithValue
        {
            ArgumentGuardHelper.ArgumentIsNotNullOrEmpty(name, nameof(name));

            if (properties.TryGetProperty(name, out var item))
            {
                return item.GetPropertyValue<T>();
            }

            return default;
        }

        public static object GetPropertyValueOrDefault<TProperty>(this IRichValueProperties<TProperty> properties, string name) where TProperty : class, IRichPropertyWithValue
        {
            ArgumentGuardHelper.ArgumentIsNotNullOrEmpty(name, nameof(name));

            if (properties.TryGetProperty(name, out var item))
            {
                return item.GetValueOrDefault();
            }

            return default;
        }

        public static string GetPropertyStringValue<TProperty>(this IRichValueProperties<TProperty> properties, string name) where TProperty : class, IRichPropertyWithValue
        {
            ArgumentGuardHelper.ArgumentIsNotNullOrEmpty(name, nameof(name));

            if (properties.TryGetProperty(name, out var item))
            {
                return item.GetStringValue();
            }

            return null;
        }

        public static int GetPropertyIntValue<TProperty>(this IRichValueProperties<TProperty> properties, string name) where TProperty : class, IRichPropertyWithValue
        {
            return properties.GetPropertyValue<TProperty, int>(name);
        }
    }

}
