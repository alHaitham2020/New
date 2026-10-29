namespace ISoft.RichReports.Editor.Internal.Fields
{
    public static class RichEditFieldHelper
    {
        public static string CalculateDisplayName(this IRichEditField _, string displayName, string parameter)
        {
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                return displayName.Trim();
            }

            return string.IsNullOrWhiteSpace(parameter)
                ? string.Empty
                : parameter.Trim();
        }

        public static string CalculateDisplayNameFormated(this IRichEditField _, string displayName, string parameter)
        {
            return string.Format("[{0}]", CalculateDisplayName(displayName, parameter));
            //return $"[ {CalculateDisplayName(displayName, parameter)} ]";
        }

        private static string CalculateDisplayName(string displayName, string parameter)
        {
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                return displayName.Trim();
            }

            return string.IsNullOrWhiteSpace(parameter)
                ? string.Empty
                : parameter.Trim();
        }
    }
}
