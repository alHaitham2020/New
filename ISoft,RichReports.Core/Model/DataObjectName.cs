using DevExpress.Data.Native;

namespace ISoft.RichReports.Core.Model
{
    public class DataObjectName
    {
        private readonly string displayName = string.Empty;

        private readonly string name = string.Empty;

        private readonly string dataMember = string.Empty;

        public string Name => name;

        public string FullName => BindingHelper.JoinStrings(".", dataMember, name);

        public string DisplayName => displayName;

        public DataObjectName(string name, string displayName)
            : this(name, displayName, string.Empty)
        {
        }

        public DataObjectName(string name, string displayName, string dataMember)
        {
            this.name = name;
            this.displayName = displayName;
            this.dataMember = dataMember;
        }

        public override bool Equals(object obj)
        {
            if (obj is DataObjectName objectName)
            {
                if (name == objectName.name)
                {
                    return displayName == objectName.displayName;
                }

                return false;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
