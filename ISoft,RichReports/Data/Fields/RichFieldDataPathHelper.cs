using System;

namespace ISoft_RichReports.Data.Fields
{
    public static class RichFieldDataPathHelper
    {
        public static string BuildPath(string path, string name)
        {
            return string.IsNullOrWhiteSpace(path) ? name : $"{path}.{name}";
        }

        public static string GetParentPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            // Handle both forward and backward slashes
            var index = path.Contains(".")
                ? path.LastIndexOf(".", StringComparison.Ordinal)
                : path.LastIndexOf(".", StringComparison.Ordinal);

            return index != -1 ? path.Substring(0, index) : path;
        }

        public static string Combine(string path, string name)
        {
            if (string.IsNullOrEmpty(path))
            {
                return name;
            }

           return StringExtensions.Concat(path, name,'.');
        }
    }
}
