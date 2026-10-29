namespace ISoft.RichReports.Editor.Preview
{
    public class RichDocumentPreviewServerFactory : IRichTemplatePreviewServerFactory
    {
        private readonly IRichServiceContainer _serviceContainer;

        public RichDocumentPreviewServerFactory(IRichServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

        public IRichDocumentPreviewServer Create()
        {
            return RichDocumentPreviewServer.Create(_serviceContainer);
        }
    }
}
