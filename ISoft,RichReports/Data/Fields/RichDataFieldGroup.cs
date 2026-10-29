using ISoft.RichReports.Fields;
using System;

namespace ISoft.RichReports.Data.Fields
{
    public class RichDataFieldGroup : RichFieldGroupBase, IRichDataFieldGroup
    {
        #region Fields
        private readonly RichDataFieldCollection _fields;
        #endregion

        #region Proprites
        public IRichDataFieldCollection Fields => _fields;

        public new IRichDataFieldGroup ParentGroup
        {
            get
            {
                return base.ParentGroup as IRichDataFieldGroup;
            }
            set
            {
                base.ParentGroup = value;
            }
        }

        #endregion

        public RichDataFieldGroup(
            string name,
            string displayName,
            string path,
            Type dataType = null,
            IRichDataFieldGroup parentGroup = null)
            : base(name, displayName, path, RichFieldGroupCategory.DataFields, dataType, parentGroup)
        {
            _fields = new ();
        }
    }

}
