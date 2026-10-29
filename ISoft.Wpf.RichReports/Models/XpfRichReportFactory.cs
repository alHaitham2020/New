namespace ISoft.Wpf.RichReports.Models
{
    /// <summary>
    /// Factory for creating report instances
    /// </summary>
    public static class XpfRichReportFactory
    {
        /// <summary>
        /// Creates a desktop report instance
        /// </summary>
        public static IXpfRichReport CreateReport(RichReportContextDependencies dependencies)
        {
            var context = RichReportContextBuilder.Build(dependencies);

            return new XpfRichReport(context);
        }

        /// <summary>
        /// Creates a desktop report instance
        /// </summary>
        public static IXpfRichReport CreateReport(
            IReportInfo reportInfo,
            IRichReportDataSource dataSource,
            IRichReportTemplateStorage storageService,
            IRichReportPrintPermissionInfo permissionService,
            IRichReportProperties properties,
            IRichDefaultTemplateInfoProvider defaultTemplateInfoProvider,
            IRichReportTemplateInfoProvider templateProvider,
            IRichReportCalculatedFieldInfoProvider calculatedFieldInfoProvider = null)
        {
            var context = RichReportContextBuilder
                            .Create(reportInfo, dataSource)
                            .WithTemplateStorage(storageService)
                            .WithPermissionsInfo(permissionService)
                            .WithProperties(properties)
                            .WithDefaultTemplateInfoProvider(defaultTemplateInfoProvider)
                            .WithTemplateInfoProvider(templateProvider)
                            .WithCalculatedFieldInfoProvider(calculatedFieldInfoProvider)
                            .Build();

            return new XpfRichReport(context);
        }

        /// <summary>
        /// Creates a desktop report instance
        /// </summary>
        public static IXpfRichReport CreateReport(
            IReportInfo reportInfo,
            IRichReportDataSource dataSource,
            IRichReportTemplateService templateService,
            IRichReportTemplateInfoProvider templateProvider,
            IRichReportPrintPermissionInfo permissionService,
            IRichReportProperties properties,
            IRichTemplateRegistry registry,
            IRichReportCalculatedFieldInfoProvider calculatedFieldInfoProvider = null)
        {
            var defaultTemplateProvider = new RichRegistryDefaultTemplateProvider(registry);

           return CreateReport(reportInfo,
                               dataSource,
                               new RichReportTemplateStorage(reportInfo, templateService),
                               permissionService,
                               properties,
                               defaultTemplateProvider,
                               templateProvider,
                               calculatedFieldInfoProvider);
        }

        /// <summary>
        /// Creates a desktop report instance
        /// </summary>
        public static IXpfRichReport CreateReport(
            IReportInfo reportInfo,
            IRichReportDataSource dataSource,
            IRichReportTemplateService templateService,
            IRichDefaultTemplateInfoProvider defaultTemplateInfoProvider,
            IRichReportTemplateInfoProvider templateProvider,
            IRichReportPrintPermissionInfo permissionService,
            IRichReportProperties properties,
            IRichReportCalculatedFieldInfoProvider calculatedFieldInfoProvider = null)
        {
            return CreateReport(reportInfo,
                                dataSource,
                                new RichReportTemplateStorage(reportInfo, templateService),
                                permissionService,
                                properties,
                                defaultTemplateInfoProvider,
                                templateProvider,
                                calculatedFieldInfoProvider);
        }
    }
}
