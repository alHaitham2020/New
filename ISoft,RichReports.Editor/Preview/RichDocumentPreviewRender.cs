using DevExpress.Office.Services;
using DevExpress.XtraRichEdit;
using ISoft.RichEdit.Native;
using ISoft.RichReports.Core.Model;
using ISoft.RichReports.Core.Templates;
using ISoft.RichReports.Editor.Services;
using ISoft.Utils.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Preview
{
    public interface IRichDocumentPreviewRender
    {
        Task LoadTemplateAsync(IRichReportTemplate templateSource, DocumentFormat? format = null);

        byte[] SaveTemplate(DocumentFormat? format = null);

        Task<byte[]> SaveTemplateAsync(DocumentFormat? format = null);

        Task SaveTemplateAsync(Stream stream, DocumentFormat? format = null);

        Task SaveTemplateAsync(string fileName, DocumentFormat? format = null);

        void Dispose();
    }

    /// <summary>
    /// Main template processor that coordinates all services
    /// </summary>
    public class RichDocumentPreviewRender : IRichDocumentPreviewRender
    {
        #region Fields
        private readonly IRichServiceContainer _serviceContainer;
        private readonly RichDocumentServer _sourceServer;
        private readonly RichDocumentServer _targetServer;

        private bool _isTemplateLoaded;
        #endregion

        #region Constructor
        protected RichDocumentPreviewRender(IRichServiceContainer serviceContainer)
        {
            ArgumentGuardHelper.ArgumentNotNull(serviceContainer, nameof(serviceContainer));

            _serviceContainer = serviceContainer;
            _sourceServer = new RichDocumentServer();
            _targetServer = new RichDocumentServer();

            _isTemplateLoaded = false;
            RegisterServices();
        }

        public static RichDocumentPreviewRender Create(IRichServiceContainer serviceContainer)
            => new RichDocumentPreviewRender(serviceContainer);

        #endregion

        #region Public Methods
        public async Task LoadTemplateAsync(IRichReportTemplate templateSource, DocumentFormat? format = null)
        {
            if (templateSource == null)
                throw new ArgumentNullException(nameof(templateSource));

            _isTemplateLoaded = false;
            try
            {
                var templateContent = templateSource.Content ?? throw new ArgumentNullException(nameof(templateSource));
                format ??= DocumentFormat.Rtf;

                // Load template into server
                _isTemplateLoaded = await _sourceServer.LoadDocumentTemplateAsync(templateContent, format.Value);

                if (_isTemplateLoaded)
                {
                    UpdateDocumentProperties(templateSource);
                    UpdateCustomProprties();

                    await _serviceContainer
                             .GetService<IRichEditDocumentRenderingService>()
                             .RenderAsync(_sourceServer.Document);
                }
                else
                {
                    throw new InvalidOperationException("faild to load a template content");
                }
            }
            catch (Exception)
            {
                _isTemplateLoaded = false;
                throw;
            }
        }

        /// <summary>
        /// Saves the rendered template content to a byte array in the specified file format. 
        /// </summary>
        public byte[] SaveTemplate(DocumentFormat? format = null)
        {
            EnsureTemplateLoaded();

            format ??= DocumentFormat.Rtf;
            return _sourceServer.SaveDocument(format.Value);
        }

        /// <summary>
        /// Saves the rendered template content to a byte array in the specified file format. 
        /// </summary>
        public async Task<byte[]> SaveTemplateAsync(DocumentFormat? format = null)
        {
            EnsureTemplateLoaded();

            format ??= DocumentFormat.Rtf;
            return await _sourceServer.SaveDocumentAsync(format.Value);
        }

        /// <summary>
        /// Saves the rendered template content to a byte array in the specified file format. 
        /// </summary>
        public async Task SaveTemplateAsync(string fileName, DocumentFormat? format = null)
        {
            EnsureTemplateLoaded();

            format ??= DocumentFormat.Rtf;
            await _sourceServer.SaveDocumentAsync(fileName, format.Value);
        }

        /// <summary>
        /// Saves the rendered template content to a byte array in the specified file format. 
        /// </summary>
        public async Task SaveTemplateAsync(Stream stream, DocumentFormat? format = null)
        {
            EnsureTemplateLoaded();

            format ??= DocumentFormat.Rtf;
            await _sourceServer.SaveDocumentAsync(stream, format.Value);
        }

        #endregion

        #region Private Methods
        private void RegisterServices()
        {
            _serviceContainer.CopyRequiredPreviewServices(_sourceServer);
            _serviceContainer.CopyRequiredPreviewServices(_targetServer);

            RegisterUriStreamProviders();
        }

        private void EnsureTemplateLoaded()
        {
            if (!_isTemplateLoaded)
                throw new InvalidOperationException("Template is not loaded. Call LoadTemplate first.");
        }

        protected virtual void RegisterUriStreamProviders()
        {
            var streamService = _sourceServer.GetService<IUriStreamService>();
            if (streamService != null)
            {
                var registrationService = _sourceServer.GetService<IRichEditUriStreamRegistrationService>();
                if (registrationService != null)
                {
                    registrationService.RegisterUriStreamProviders(streamService);
                }
            }
        }

        protected virtual void UpdateCustomProprties()
        {
            var service = _serviceContainer.GetService<IRichEditDocumentCustomPropertiesManagerService>();
            if (service != null)
            {
                _sourceServer.Document.ExecuteOperation(d =>
                {
                    service.UpdateDocCustomProperties(d.CustomProperties);
                });
            }
        }

        protected virtual void UpdateDocumentProperties(IRichReportTemplate template)
        {
            var service = _serviceContainer.GetService<IRichEditDocumentPropertiesManagerService>();
            if (service != null)
            {
                _sourceServer.Document.ExecuteOperation(d =>
                {
                    service.UpdateDocPreviewProperties(d.DocumentProperties, template);
                });
            }
        }

        #endregion

        #region IDisposable
        public void Dispose()
        {
            _sourceServer?.Dispose();
            _targetServer?.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
