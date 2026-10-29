using DevExpress.Office.Services;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using ISoft.RichEdit.Native;
using ISoft.RichReports.Core.Model;
using ISoft.RichReports.Core.Templates;
using ISoft.RichReports.Editor.Services;
using ISoft.Utils.Extentions;
using ISoft.Utils.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Preview
{
    /// <summary>
    /// Main template processor that coordinates all services
    /// </summary>
    public class RichDocumentPreviewServer : IRichDocumentPreviewServer
    {
        #region Fields
        private readonly IRichServiceContainer _serviceContainer;
        private readonly RichDocumentServer _sourceServer;
        private readonly RichDocumentServer _targetServer;

        private bool _isTemplateLoaded;
        private bool _isMailMergeExecuted;
        #endregion

        #region Constructor
        protected RichDocumentPreviewServer(IRichServiceContainer serviceContainer)
        {
            ArgumentGuardHelper.ArgumentNotNull(serviceContainer, nameof(serviceContainer));
            
            _serviceContainer = serviceContainer;
            _sourceServer = new RichDocumentServer();
            _targetServer = new RichDocumentServer();

            _isMailMergeExecuted = false;
            _isTemplateLoaded = false;
            RegisterServices();
            SubscribeEvents();
        }

        public static IRichDocumentPreviewServer Create(IRichServiceContainer serviceContainer) 
            => new RichDocumentPreviewServer(serviceContainer);

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

        public async Task FillDataAsync(object dataSource, DevExpress.XtraRichEdit.API.Native.MailMergeOptions options = null)
        {
            EnsureTemplateLoaded();

            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));

            _isMailMergeExecuted = false;
            try
            {
                options ??= _sourceServer.CreateMailMergeOptions();
                options.DataSource = dataSource;

                // Execute mail merge using the service
               await _serviceContainer
                        .GetService<IRichEditMailMergeExecutionService>()
                        .ExecuteMailMergeAsync(_sourceServer, options, _targetServer);

                _isMailMergeExecuted = true;
            }
            catch (Exception)
            {
                _isMailMergeExecuted = false;
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

        /// <summary>
        /// Shows print preview
        /// </summary>
        public void ShowPrintPreview(string title = null)
        {
            //EnsureTemplateLoaded();
            //EnsureMailMergeExecuted();

            ExecutePrintOperation((service, server) => service.ShowPrintPreview(server, title));

            //_serviceContainer
            //    .GetService<IRichEditDocumentPrintingService>()
            //    .ShowPrintPreview(_targetServer, title);
        }

        /// <summary>
        /// Shows print dialog
        /// </summary>
        public void ShowPrintDialog()
        {
            //EnsureTemplateLoaded();
            //EnsureMailMergeExecuted();

            ExecutePrintOperation((service, server) => service.ShowPrintDialog(server));

            //_serviceContainer
            //    .GetService<IRichEditDocumentPrintingService>()
            //    .ShowPrintDialog(_targetServer);
        }

        public void Print()
        {
            //EnsureTemplateLoaded();
            //EnsureMailMergeExecuted();

            ExecutePrintOperation((service, server) => service.Print(server));

            //_serviceContainer
            //    .GetService<IRichEditDocumentPrintingService>()
            //    .Print(_targetServer);
        }

        public void Export(ExportFormat format, string defaultFileName = null)
        {
            //EnsureTemplateLoaded();
            //EnsureMailMergeExecuted();

            ExecutePrintOperation((service, server) => service.Export(server, format, defaultFileName));

            //_serviceContainer
            //    .GetService<IRichEditDocumentPrintingService>()
            //    .Export(_targetServer, format, defaultFileName);
        }

        #endregion

        #region Private Methods
        private void RegisterServices()
        {
            _serviceContainer.CopyRequiredPreviewServices(_sourceServer);
            _serviceContainer.CopyRequiredPreviewServices(_targetServer);

            RegisterUriStreamProviders();
        }

        private void SubscribeEvents()
        {
            var mailMerageClientService = _serviceContainer.GetService<IRichMailMergeClientService>();

            if (mailMerageClientService != null)
            {
                _sourceServer.MailMergeStarted += (s, e) =>
                {
                    mailMerageClientService.Started(e);
                };

                _sourceServer.MailMergeFinished += (s, e) =>
                {
                    mailMerageClientService.Finished(e);
                };

                _sourceServer.MailMergeRecordStarted += (s, e) =>
                {
                    mailMerageClientService.RecordStarted(e);
                };

                _sourceServer.MailMergeRecordFinished += (s, e) =>
                {
                    mailMerageClientService.RecordFinished(e);
                };
            }

            var variableManagerService = _serviceContainer.GetService<IRichEditDocumentVariableManagerService>();

            if (variableManagerService != null)
            {
                _targetServer.CalculateDocumentVariable += (s, e) =>
                {
                    variableManagerService.Calculate(e);
                };
            }
        }

        private void EnsureTemplateLoaded()
        {
            if (!_isTemplateLoaded)
                throw new InvalidOperationException("Template is not loaded. Call LoadTemplate first.");
        }

        private void EnsureMailMergeExecuted()
        {
            if (!_isMailMergeExecuted)
                throw new InvalidOperationException("MailMerge is not executed. Call ExecuteMailMerge first.");
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
                _sourceServer
                    .Document
                    .ExecuteOperation(d => service.UpdateDocCustomProperties(d.CustomProperties));
            }
        }

        protected virtual void UpdateDocumentProperties(IRichReportTemplate template)
        {
            var service = _serviceContainer.GetService<IRichEditDocumentPropertiesManagerService>();
            if (service != null)
            {
                _sourceServer
                    .Document
                    .ExecuteOperation(d => service.UpdateDocPreviewProperties(d.DocumentProperties, template));
            }
        }

        protected void ExecutePrintOperation(Action<IRichEditDocumentPrintingService, RichDocumentServer> action)
        {
            EnsureTemplateLoaded();
            EnsureMailMergeExecuted();

            _ = _serviceContainer
                .GetService<IRichEditDocumentPrintingService>()
                .DoIfNotNull((service) => action(service, _targetServer));
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

        //public void Render(object dataSource)
        //{
        //    //DataSource = dataSource;

        //    //StreamSerializationHelper.DeserializeFromBytes(File.ReadAllBytes(@"D:\RenderTemplate.RichRpt"), s =>
        //    //{
        //    //    var temp = JsonSerializationHelper.DeserializeFromStream<RichReportTemplate>(s);
        //    //    DocumentTemplate = temp;
        //    //});

        //    //Fields = RichDataFieldsReflectionBuilder.Default.BuildFromObject(DataSource);
        //    //var temp = RichReportTemplateSerializationHelper.DeserializeFromFile(@"D:\RenderTemplate.RichRpt");
        //    ////_editor.RegisterServices();
        //    //DocumentTemplate = temp;

        //    //var sb = FluentStringBuilder.CreateNew();

        //    //foreach(var f in Fields)
        //    //{
        //    //    sb.AppendLine($"path : {f.Path} DataType : {f.DataType}");

        //    //    foreach(var ff in f.Fields)
        //    //    {
        //    //        sb.AppendLine($"path : {ff.Path} DataType : {ff.DataType}");
        //    //    }
        //    //}

        //    //RunTimeLogger.CurrentLogger.LogInfo(sb.ToString());

        //    //_editor.RegisterServices();

        //    //StreamSerializationHelper.DeserializeFromBytes(temp.Content, s =>
        //    //{
        //    //    _editor.LoadDocument(s, DocumentFormat.Docm);
        //    //    _editor.RegisterServices();
        //    //});


        //    //RichDocumentTemplateProcessor processor = new();
        //    //processor.LoadTemplate(temp.Content);
        //    //processor.ExecuteMailMerge(DataSource);

        //    //newTemp.Content = processor.GetRenderedTemplateContent();

        //    //RichReportEngine engine = new();
        //    //engine.Print(temp.Content, DataSource);

        //    //var result = engine.ProcessAndRender(temp.Content);
        //    //newTemp.Content = result;

        //    //StreamSerializationHelper.DeserializeFromBytes(newTemp.Content, s =>
        //    //{
        //    //    _editor.LoadDocumentTemplate(s);
        //    //});

        //    //_editor.Document.Fields.Update();

        //    //FieldManagementService fieldManagementService = new FieldManagementService();
        //    //var count = fieldManagementService.ScanDocumentFields(_editor.Document).ToRowFormateString(x => x.FieldCode.BuildFieldCode());
        //    //NotificationManagerHelper.ShowAlert($"{count}");

        //    //var temps = new RichReportTemplate
        //    //{
        //    //    Content = null,
        //    //    Thumbnail = null,
        //    //    Data = StreamSerializationHelper.SerializeToString(s =>
        //    //        {
        //    //            _editor.SaveDocument(s, DocumentFormat.OpenXml);
        //    //        })
        //    //};


        //    //StreamSerializationHelper.SerializeToFile(s =>
        //    //{
        //    //    _editor.SaveDocument(s, DocumentFormat.FlatOpcMacroEnabled);
        //    //}, @"D:\FlatOpcMacroEnabled.RichRpt");

        //    //StreamSerializationHelper.SerializeToFile(s =>
        //    //{
        //    //    _editor.SaveDocument(s, DocumentFormat.Rtf);
        //    //}, @"D:\Rtf.RichRpt");

        //    //StreamSerializationHelper.SerializeToFile(s =>
        //    //{
        //    //    _editor.SaveDocument(s, DocumentFormat.FlatOpcMacroEnabledTemplate);
        //    //}, @"D:\FlatOpcMacroEnabledTemplate.RichRpt");

        //    //StreamSerializationHelper.SerializeToFile(s =>
        //    //{
        //    //    _editor.SaveDocument(s, DocumentFormat.FlatOpc);
        //    //}, @"D:\FlatOpc.RichRpt");

        //    //StreamSerializationHelper.SerializeToFile(s =>
        //    //{
        //    //    _editor.SaveDocument(s, DocumentFormat.FlatOpcTemplate);
        //    //}, @"D:\FlatOpcTemplate.RichRpt");

        //    //RichReportTemplateSerializationHelper.SerializeToFile(temps, @"D:\RenderTemplate22.RichRpt");
        //}

    }
}
