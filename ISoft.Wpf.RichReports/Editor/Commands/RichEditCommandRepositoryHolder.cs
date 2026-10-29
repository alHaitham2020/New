using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Services.Implementation;
using ISoft.Utils.Extentions;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public class RichEditCommandRepositoryHolder : XpfRichEditCommandRepositoryHolder
    {
        //public new IXpfRichEditCommandRepository<IRichEditControl> CommandRepository => base.CommandRepository as IXpfRichEditCommandRepository<IRichEditControl>;

        protected override IRichEditCommandRepository<IRichEditControl> CreateCommandRepositoryCore()
        {
            return new XpfRichEditCommandRepository<IRichEditControl>();
        }

        protected override void PopulateCommandRepository(IRichEditCommandRepository<IRichEditControl> table)
        {
            base.PopulateCommandRepository(table);

            table.AddCommand(XpfRichEditCommandId.ToolsFieldsCommandGroup, owner => new ToolsFieldsCommandGroup(owner));
            table.AddCommand(XpfRichEditCommandId.RemoveSelectedField, owner => new RemoveSelectedFieldCommand(owner));
            table.AddCommand(XpfRichEditCommandId.RemoveSelectedFields, owner => new RemoveSelectedFieldsCommand(owner));
            table.AddCommand(XpfRichEditCommandId.ChangeFieldBarcodeType, owner => new ChangeFieldBarcodeTypeCommand(owner));
            table.AddCommand(XpfRichEditCommandId.ChangeFieldImageWidth, owner => new ChangeFieldImageWidthCommand(owner));
            table.AddCommand(XpfRichEditCommandId.ChangeFieldImageHeight, owner => new ChangeFieldImageHeightCommand(owner));
            table.AddCommand(XpfRichEditCommandId.ChangeFieldDateTimeFormat, owner => new ChangeFieldDateTimeFormatCommand(owner));
            table.AddCommand(XpfRichEditCommandId.ChangeFieldNumericFormat, owner => new ChangeFieldNumericFormatCommand(owner));
            table.AddCommand(XpfRichEditCommandId.RichShowCalculatedFieldsEditorForm, owner => new RichShowCalculatedFieldsEditorFormCommand(owner));

            table.TryCast<IXpfRichEditCommandRepository<IRichEditControl>>()
                  .DoIfNotNull(r =>
                  {
                      r.UpdateCommand(RichEditCommandId.FileSave, owner => new RichSaveDocumentCommand(owner));
                      r.UpdateCommand(RichEditCommandId.PrintPreview, owner => new RichPrintPreviewCommand(owner));
                  });
        }
    }

}
