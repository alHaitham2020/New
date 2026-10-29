using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Services.Implementation;
using ISoft.RichReportDesigner.UI.RichEdit.Commands;

namespace ISoft.RichReportDesigner.UI.RichEdit.Services
{
    public class RichEditorCommandsFactoryService : XpfRichEditCommandFactoryService
    {
        public RichEditorCommandsFactoryService(IRichEditControl control) : base(control)
        {

        }

        protected override IRichEditCommandRepository CommandRepository
        {
            get
            {
                return CommandRepositoryHolderBase<RichEditCommandRepositoryHolder, IRichEditControl>.Instance.CommandRepository;
            }
        }
    }

}
