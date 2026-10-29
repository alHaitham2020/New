namespace ISoft.RichReports.Core.Client
{
    public enum RichReportTemplateSourceType
    {
        /// <summary>
        /// User's templates + shared templates
        /// </summary>
        PrintTemplates,

        /// <summary>
        /// Only user's templates
        /// </summary>
        UserOwnedTemplates,

        /// <summary>
        /// All available templates
        /// </summary>
        AllTemplates
    }
}
