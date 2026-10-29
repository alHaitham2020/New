using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.DataAnnotations;
using System;

namespace ISoft.RichReports.Data.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public abstract class RichDataFieldFormatAttributeBase : AttributeBase
    {
        public abstract RichFieldFormat? GetFormat();
    }
}
