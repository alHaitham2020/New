using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReportDesigner.Core.Fields.Data;
using ISoft.RichReportDesigner.Core.Fields.Display;
using ISoft.RichReportDesigner.Core.Fields.Format;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public abstract class ChangeFieldFormatCommandBase : ChangeFieldSwitchValueCommand<string>
    {
        protected ChangeFieldFormatCommandBase(IRichEditControl control) : base(control) { }

        protected ChangeFieldFormatCommandBase(IRichEditControl control, Field field) : base(control, field) { }

        protected override string DefaultValue => null;

        protected abstract RichFieldDataTypeCategory DataTypeCategory { get; }

        private IRichDisplayField displayField;

        protected internal IRichDisplayField DisplayField => displayField;

        protected sealed override bool CanChangeFieldCode(Field field)
        {
            return DisplayField?.DataTypeCategory == DataTypeCategory;
        }

        protected sealed override void ChangeValueCore(IValueBasedCommandUIState<string> state)
        {
            var dsField = DisplayField;
            if (dsField != null && dsField.DataTypeCategory == DataTypeCategory)
            {
                UpdateSwitchValue(Field, state);
            }
        }

        private bool ValidateDisplayField()
        {
            if (displayField != null)
            {
                return true;
            }

            displayField = GetDisplayField(Field);
            return displayField != null;
        }

        protected sealed override bool ValidateField()
        {
            return base.ValidateField() && ValidateDisplayField();
        }
    }
}
