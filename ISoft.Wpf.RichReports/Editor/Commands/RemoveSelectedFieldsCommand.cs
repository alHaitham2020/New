namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class RemoveSelectedFieldsCommand : SelectionBasedPropertyChangeCommandBase
    {
        private Field firstField;
        private Field lastField;
        private RichDocumentModelFieldHelper _helper;

        public override RichEditCommandId Id => XpfRichEditCommandId.RemoveSelectedFields;
        public override XtraRichEditStringId MenuCaptionStringId => XtraRichEditStringId.MenuCmd_None;
        public override XtraRichEditStringId DescriptionStringId => XtraRichEditStringId.MenuCmd_None;

        internal RichDocumentModelFieldHelper Helper =>
            _helper ??= new RichDocumentModelFieldHelper(DocumentModel);

        public RemoveSelectedFieldsCommand(IRichEditControl control) : base(control) { }

        protected override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state)
        {
            return DocumentModelChangeActions.None;
        }

        protected override void ModifyDocumentModelCore(ICommandUIState state)
        {
            if (firstField == null || lastField == null)
                return;

            var fields = GetFieldsToDelete();
            Helper.DeleteFieldCodes(fields);
        }

        protected override void UpdateUIStateCore(ICommandUIState state)
        {
            CheckExecutedAtUIThread();

            state.Enabled = IsContentEditable;
            if (!state.Enabled)
            {
                state.Visible = true;
                return;
            }

            ValidateFields();

            state.Enabled = (firstField != null && lastField != null);
            state.Visible = true;
        }

        protected virtual void ValidateFields()
        {
            firstField = null;
            lastField = null;

            var selection = DocumentModel.Selection;

            var interval = selection.GetRunInterval(ActivePieceTable);
            if (interval == null) return;

            RunIndex startRunIndex = interval.Start;
            RunIndex endRunIndex = interval.End;

            firstField = Helper.FindFirstField(startRunIndex);
            if (firstField == null)
                return;

            lastField = Helper.FindLastField(endRunIndex) ?? firstField;

            if (firstField.FirstRunIndex > endRunIndex || startRunIndex > lastField.LastRunIndex)
            {
                firstField = null;
                lastField = null;
            }
        }

        private List<Field> GetFieldsToDelete()
        {
            List<Field> list = new();
            for (int i = firstField.Index; i <= lastField.Index; i++)
            {
                if (ActivePieceTable.Fields[i].Parent == null)
                {
                    list.Add(ActivePieceTable.Fields[i]);
                }
            }
            return list;
        }
    }
}
