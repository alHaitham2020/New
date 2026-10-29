using ISoft.Utils.DataAnnotations;
using System;

namespace ISoft.RichReports.Editor.Fields.Instructions
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RichFieldArgumentAttribute : AttributeBase
    {
        private readonly int _index;

        public int Index => _index;

        public RichFieldArgumentAttribute(int index)
        {
            _index = index;
        }
    }
}
