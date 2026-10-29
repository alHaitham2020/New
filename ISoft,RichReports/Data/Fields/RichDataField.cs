using ISoft.RichReports.Fields;

namespace ISoft.RichReports.Data.Fields
{
    public class RichDataField : RichFieldNodeBase, IRichDataField
    {
        public new IRichDataFieldGroup ParentGroup
        {
            get => base.ParentGroup as IRichDataFieldGroup; 
            set => base.ParentGroup = value;
        }

        public bool AllowUseDefaultEmptyValue { get; set; }

        /// <summary>
        /// if IsDetailField (is Row in table) then name else full path
        /// </summary>
        public string FieldDataName => IsDetailField ? Name : Path;

        public RichDataField()
        {

        }
    }
}
