using System;

namespace ISoft.RichReports.Editor.Fields.Instructions
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RichFieldSwitchAttribute : AttributeBase
    {
        private readonly string _key;

        public string Key => _key;

        public RichFieldSwitchAttribute(string key)
        {
            _key = key;
        }
    }
}
