using ISoft.Printing.Shared.Data;
using ISoft.RichReports.Core.Client;
using ISoft.RichReports.Data.DataSources;
using ISoft.RichReports.Editor.RichProperties;
using ISoft.Utils.Helpers;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.UI
{
    /// <summary>
    /// Fluent builder for creating report contexts with simplified API
    /// </summary>
    public class RichReportContextBuilder : IRichReportContextBuilder
    {
        #region Required Fields
        private IReportInfo _reportInfo;
        private IRichReportDataSource _dataSource;
        private IRichReportTemplateStorage _templateStorage;
        private IRichReportTemplateInfoProvider _templateInfoProvider;
        #endregion

        #region Optional Fields
        private IRichReportPrintPermissionInfo _permissionInfo;
        private IRichReportProperties _reportProperties;
        private IRichDocumentProperties _documentProperties;
        private IRichDocumentCustomProperties _customProperties;
        private IRichDocumentVariablesProperties _documentVariablesProperties;
        private IRichDefaultTemplateInfoProvider _defaultTemplateInfoProvider;
        private IRichReportCalculatedFieldInfoProvider _calculatedFieldInfoProvider;
        private IRichReportCalculatedFieldStorage _calculatedFieldStorage;
        #endregion

        #region IRichReportContextBuilder Implemintation 
        public IRichReportContextBuilder WithReportInfo(IReportInfo reportInfo)
        {
            _reportInfo = reportInfo ?? throw new ArgumentNullException(nameof(reportInfo));
            return this;
        }

        public IRichReportContextBuilder WithDataSource(IRichReportDataSource dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            return this;
        }

        public IRichReportContextBuilder WithTemplateStorage(IRichReportTemplateStorage storage)
        {
            _templateStorage = storage ?? throw new ArgumentNullException(nameof(storage));
            return this;
        }

        public IRichReportContextBuilder WithTemplateInfoProvider(IRichReportTemplateInfoProvider provider)
        {
            _templateInfoProvider = provider ?? throw new ArgumentNullException(nameof(provider));
            return this;
        }

        public IRichReportContextBuilder WithPermissionsInfo(IRichReportPrintPermissionInfo permissions)
        {
            _permissionInfo = permissions;
            return this;
        }

        public IRichReportContextBuilder WithProperties(IRichReportProperties reportProperties)
        {
            _reportProperties = reportProperties;
            return this;
        }

        public IRichReportContextBuilder WithDocumentProperties(IRichDocumentProperties documentProperties)
        {
            _documentProperties = documentProperties;
            return this;
        }

        public IRichReportContextBuilder WithCustomProperties(IRichDocumentCustomProperties customProperties)
        {
            _customProperties = customProperties;
            return this;
        }

        public IRichReportContextBuilder WithVariableProperties(IRichDocumentVariablesProperties variableProperties)
        {
            _documentVariablesProperties = variableProperties;
            return this;
        }

        public IRichReportContextBuilder WithDefaultTemplateInfoProvider(IRichDefaultTemplateInfoProvider provider)
        {
            _defaultTemplateInfoProvider = provider;
            return this;
        }

        public IRichReportContextBuilder WithCalculatedFieldInfoProvider(IRichReportCalculatedFieldInfoProvider provider)
        {
            _calculatedFieldInfoProvider = provider;
            return this;
        }

        public IRichReportContext Build()
        {
            ValidateRequiredFields();

            // Initialize defaults for optional fields
            _permissionInfo ??= new DefaultRichReportPrintPermissionInfo();
            _documentProperties ??= new RichDocumentProperties();
            _customProperties ??= new RichDocumentCustomProperties();
            _documentVariablesProperties??= new RichDocumentVariablesProperties();
            _reportProperties ??= new RichReportProperties();

            _calculatedFieldStorage = new RichReportCalculatedFieldStorage(_reportInfo, _calculatedFieldInfoProvider);

            BuildReportProperties(_customProperties, _reportProperties);

            return new RichReportContext
            {
                ReportInfo = _reportInfo,
                DataSource = _dataSource,
                TemplateStorage = _templateStorage,
                TemplateInfoProvider = _templateInfoProvider,
                PermissionInfo = _permissionInfo,
                Properties = _reportProperties,
                DocumentProperties = _documentProperties,
                DocumentCustomProperties = _customProperties,
                DocumentVariableProperties = _documentVariablesProperties,
                DefaultTemplateInfoProvider = _defaultTemplateInfoProvider,
                CalculatedFieldStorage = _calculatedFieldStorage,
            };
        }
        #endregion

        #region Helpers

        protected virtual void BuildReportProperties(IRichDocumentCustomProperties customProperties, IRichReportProperties reportProperties)
        {
            // Add report properties to custom properties
            ReportCustomPropertiesBuilder.AddReportProperties(customProperties, reportProperties);
        }

        private void ValidateRequiredFields()
        {
            var missing = new List<string>();

            if (_reportInfo is null) missing.Add(nameof(_reportInfo));
            if (_dataSource is null) missing.Add(nameof(_dataSource));
            if (_templateStorage is null) missing.Add(nameof(_templateStorage));
            if (_templateInfoProvider is null) missing.Add(nameof(_templateInfoProvider));

            if (missing.Count != 0)
                throw new InvalidOperationException($"Required fields missing: {string.Join(", ", missing)}");
        }

        /// <summary>
        /// Creates a builder with required dependencies
        /// </summary>
        public static IRichReportContextBuilder Create(IReportInfo reportInfo, IRichReportDataSource dataSource) =>
            new RichReportContextBuilder()
                .WithReportInfo(reportInfo)
                .WithDataSource(dataSource);

        public static IRichReportContext Build(RichReportContextDependencies dependencies)
        {
            ArgumentGuardHelper.ArgumentNotNull(dependencies, nameof(dependencies));

            return Create(dependencies.ReportInfo, dependencies.DataSource)
                      .WithTemplateStorage(dependencies.TemplateStorage)
                      .WithPermissionsInfo(dependencies.PermissionInfo)
                      .WithProperties(dependencies.Properties)
                      .WithDefaultTemplateInfoProvider(dependencies.DefaultTemplateInfoProvider)
                      .WithTemplateInfoProvider(dependencies.TemplateInfoProvider)
                      .WithCalculatedFieldInfoProvider(dependencies.CalculatedFieldInfoProvider)
                      .Build();
        }

        #endregion
    }
}
