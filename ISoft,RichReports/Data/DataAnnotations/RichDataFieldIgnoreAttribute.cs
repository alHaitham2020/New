using ISoft.Utils.DataAnnotations;
using System;

namespace ISoft.RichReports.Data.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RichDataFieldIgnoreAttribute : AttributeBase
    {

    }
}
