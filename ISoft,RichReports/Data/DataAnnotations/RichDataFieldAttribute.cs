using ISoft.Utils.DataAnnotations;
using System;

namespace ISoft.RichReports.Data.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RichDataFieldAttribute : AttributeBase
    {
        public object NullValue { get; set; }

        public bool AllowUseDefaultEmptyValue { get; set; } = true;

        public bool AllowUseDefaultFormat { get; set; } = true;
    }
}
