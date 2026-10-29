using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReportDesigner.Core.Fields.Data;
using ISoft.RichReportDesigner.Core.Fields.Switches;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class ChangeFieldDateTimeFormatCommand : ChangeFieldFormatCommandBase
    {
        public ChangeFieldDateTimeFormatCommand(IRichEditControl control) : base(control)
        {
        }

        public ChangeFieldDateTimeFormatCommand(IRichEditControl control, Field field) : base(control, field)
        {

        }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldDateTimeFormat;

        protected override string SwitchKey => RichStandardFieldSwitchKeys.DateTimeKey;

        protected override RichFieldDataTypeCategory DataTypeCategory => RichFieldDataTypeCategory.DateTime;
    }
}
