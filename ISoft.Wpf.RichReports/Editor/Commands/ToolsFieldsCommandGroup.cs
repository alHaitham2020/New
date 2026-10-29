namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class ToolsFieldsCommandGroup : FieldBasedRichEditMenuItemSimpleCommand
    {
        public ToolsFieldsCommandGroup(IRichEditControl control)
         : this(control, null)
        {

        }

        public ToolsFieldsCommandGroup(IRichEditControl control, Field commandField) : base(control, commandField)
        {

        }

        public override RichEditCommandId Id => XpfRichEditCommandId.ToolsFieldsCommandGroup;

        public override XtraRichEditStringId MenuCaptionStringId => XtraRichEditStringId.MenuCmd_None;

        public override XtraRichEditStringId DescriptionStringId => XtraRichEditStringId.MenuCmd_None;

        protected override void ExecuteCore()
        {
            
        }

        protected override void UpdateUIStateCore(ICommandUIState state)
        {
            CheckExecutedAtUIThread();
            ValidateField();

            state.Visible = Field != null;
        }
    }

}
