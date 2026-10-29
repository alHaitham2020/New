using System;

namespace ISoft.RichReports.Data.Annotations
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public abstract class RichDataFieldFormatAttributeBase : AttributeBase
    {
        public abstract RichFieldFormat? GetFormat();
    }
}
