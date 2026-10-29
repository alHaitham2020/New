using ISoft.Wpf.RichReports.Editor.Commands;

namespace ISoft.Wpf.RichReports.Editor.Services
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
