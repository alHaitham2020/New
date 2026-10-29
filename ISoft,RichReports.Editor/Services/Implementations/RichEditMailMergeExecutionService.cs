using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using ISoft.RichReports.Editor.MailMerge;
using ISoft.Utils.Helpers;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services.Implementation
{
    /// <summary>
    /// Service responsible for executing mail merge operations
    /// </summary>
    public class RichEditMailMergeExecutionService : IRichEditMailMergeExecutionService
    {
        public RichEditMailMergeExecutionService()
        {

        }

        public void ExecuteMailMerge(Document sourceDocument, MailMergeOptions options, Document targetDocument)
        {
            ArgumentGuardHelper.ArgumentNotNull(sourceDocument, nameof(sourceDocument));
            ArgumentGuardHelper.ArgumentNotNull(options, nameof(options));
            ArgumentGuardHelper.ArgumentNotNull(targetDocument, nameof(targetDocument));

            try
            {
                sourceDocument.MailMerge(options, targetDocument);
            }
            catch (Exception ex)
            {
                throw new MailMergeException("Failed to execute mail merge", ex);
            }
        }

        public void ExecuteMailMerge(IRichEditDocumentServer inputServer, MailMergeOptions options, IRichEditDocumentServer outputServer)
        {
            ArgumentGuardHelper.ArgumentNotNull(inputServer, nameof(inputServer));
            ArgumentGuardHelper.ArgumentNotNull(options, nameof(options));
            ArgumentGuardHelper.ArgumentNotNull(outputServer, nameof(outputServer));

            try
            {
                ExecuteMailMerge(inputServer.Document, options, outputServer.Document);
            }
            catch (Exception ex)
            {
                throw new MailMergeException("Failed to execute mail merge", ex);
            }
        }

        public async Task ExecuteMailMergeAsync(Document sourceDocument, MailMergeOptions options, Document targetDocument)
        {
            await Task.Run(() =>
            {
                ExecuteMailMerge(sourceDocument, options, targetDocument);
            });
        }

        public async Task ExecuteMailMergeAsync(IRichEditDocumentServer inputServer, MailMergeOptions options, IRichEditDocumentServer outputServer)
        {
            await Task.Run(() =>
            {
                ExecuteMailMerge(inputServer, options, outputServer);
            });
        }
    }
}
