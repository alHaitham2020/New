using ISoft.RichReports.Editor.Properties.RichProperties;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    public static class RichPropertyHelper
    {
        /// <summary>
        /// Gets the display value for a field, handling null/empty checks and image formatting.
        /// </summary>
        public static object GetDisplayValue(this IRichProperty property, object value)
        {
            if (property == null) return value;

            // Return null for empty/null values based on field's data type and null value definition
            if (property.AllowUseDefaultEmptyValue && DataValidationHelper.IsEmptyValue(value, property.PropertyType, property.DefaultValue, false))
                return null;

            return DisplayValueHelper.GetDisplayValue(value);
        }

        /// <summary>
        /// Gets the display value for a field, handling null/empty checks and image formatting.
        /// </summary>
        public static object GetDisplayValue(this IRichPropertyWithValue property)
        {
            if (property == null) return null;

            return property.GetDisplayValue(property.GetValueOrDefault());
        }
    }
}
