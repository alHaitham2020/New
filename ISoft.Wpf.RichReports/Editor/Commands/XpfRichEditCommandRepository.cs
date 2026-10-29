using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Services.Implementation;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public interface IXpfRichEditCommandRepository<TOwner> : IRichEditCommandRepository<TOwner> where TOwner : class, IRichEditControl
    {
        void UpdateCommand(RichEditCommandId commandId, CommandFactory<RichEditCommand, TOwner> factory);
    }

    public class XpfRichEditCommandRepository<TOwner> : RichEditCommandRepository<TOwner>, IXpfRichEditCommandRepository<TOwner> where TOwner : class, IRichEditControl
    {
        void IXpfRichEditCommandRepository<TOwner>.UpdateCommand(RichEditCommandId commandId, CommandFactory<RichEditCommand, TOwner> factory)
        {
            if (!Cache.ContainsKey(commandId)) return;

            Cache[commandId] = factory;
        }
    }

}
