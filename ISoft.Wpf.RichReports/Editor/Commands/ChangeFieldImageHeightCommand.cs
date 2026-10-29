using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReportDesigner.Core.Fields.Instructions;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class ChangeFieldImageHeightCommand : ChangeFieldSwitchValueImageDimensionCommandBase<int?>
    {
        public ChangeFieldImageHeightCommand(IRichEditControl control) : base(control) { }
        public ChangeFieldImageHeightCommand(IRichEditControl control, Field field) : base(control, field) { }

        public override RichEditCommandId Id => XpfRichEditCommandId.ChangeFieldImageHeight;

        protected override string SwitchKey => RichFieldSwitchKeys.ImageBasedField.ImageHeight;

        protected override int? DefaultValue => 0;
    }
}
