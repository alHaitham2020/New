namespace ISoft.Wpf.RichReports.Editor.Commands
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
