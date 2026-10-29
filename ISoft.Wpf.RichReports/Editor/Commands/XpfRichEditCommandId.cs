namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public class XpfRichEditCommandId
    {
        private const int StartId = 5000;

        public static readonly RichEditCommandId ToolsFieldsCommandGroup = new(StartId + 1);

        public static readonly RichEditCommandId RemoveSelectedField = new(StartId + 2);

        public static readonly RichEditCommandId RemoveSelectedFields = new(StartId + 3);

        public static readonly RichEditCommandId ChangeFieldBarcodeType = new(StartId + 4);

        public static readonly RichEditCommandId ChangeFieldImageWidth = new(StartId + 5);

        public static readonly RichEditCommandId ChangeFieldImageHeight = new(StartId + 6);
        
        public static readonly RichEditCommandId ChangeFieldDateTimeFormat = new(StartId + 7);

        public static readonly RichEditCommandId ChangeFieldNumericFormat = new(StartId + 8);

        public static readonly RichEditCommandId RichShowCalculatedFieldsEditorForm = new(StartId + 9);
    }
}
