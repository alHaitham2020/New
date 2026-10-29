using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditMailMergeExecutionService
    {
        void ExecuteMailMerge(IRichEditDocumentServer inputServer, MailMergeOptions options, IRichEditDocumentServer outputServer);

        void ExecuteMailMerge(Document sourceDocument, MailMergeOptions options, Document targetDocument);

        Task ExecuteMailMergeAsync(IRichEditDocumentServer inputServer, MailMergeOptions options, IRichEditDocumentServer outputServer);

        Task ExecuteMailMergeAsync(Document sourceDocument, MailMergeOptions options, Document targetDocument);
    }
}
