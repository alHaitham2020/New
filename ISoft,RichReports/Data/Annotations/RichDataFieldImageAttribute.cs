using System;

namespace ISoft.RichReports.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RichDataFieldImageAttribute : AttributeBase
    {
        private readonly ImageUriSchemeType _schemeType;

        public ImageUriSchemeType SchemeType => _schemeType;

        public RichDataFieldImageAttribute(ImageUriSchemeType schemeType)
        {
            _schemeType = schemeType;
        }
    }
}
