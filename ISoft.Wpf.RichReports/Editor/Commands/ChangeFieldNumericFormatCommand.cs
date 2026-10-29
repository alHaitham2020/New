using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReportDesigner.Core.Fields.Data;
using ISoft.RichReportDesigner.Core.Fields.Switches;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class ChangeFieldNumericFormatCommand : ChangeFieldFormatCommandBase
    {
        public ChangeFieldNumericFormatCommand(IRichEditControl control) : base(control) { }

        public ChangeFieldNumericFormatCommand(IRichEditControl control, Field field) : base(control, field) { }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldNumericFormat;

        protected override string SwitchKey => RichStandardFieldSwitchKeys.NumericKey;

        protected override RichFieldDataTypeCategory DataTypeCategory => RichFieldDataTypeCategory.Numeric;
    }
}
