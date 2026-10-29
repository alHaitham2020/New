using DevExpress.XtraPrinting;
using ISoft.Printing.Shared.Data;
using ISoft.RichReports.Core.Client;
using ISoft.RichReports.Core.Model;
using ISoft.RichReports.Core.Templates;
using ISoft.RichReports.Data.DataSources;
using ISoft.RichReports.Data.Fields;
using ISoft.RichReports.Editor.RichProperties;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.UI
{
    public interface IRichReport : IRichServiceContainer
    {
        IReportInfo ReportInfo { get; }

        IRichReportDataSource DataSource { get; }

        IRichDataFieldGroupCollection DataFields { get; }

        IRichReportProperties Properties { get; }

        IRichDocumentProperties DocumentProperties { get; }

        IRichDocumentCustomProperties DocumentCustomProperties { get; }

        IRichDocumentVariablesProperties DocumentVariablesProperties { get; }

        IRichCalculatedFieldProperties CalculatedFieldProperties { get; }

        IRichReportPrintPermissionInfo PermissionInfo { get; }

        IRichReportTemplateStorage TemplateStorage { get; }

        IRichReportCalculatedFieldStorage CalculatedFieldStorage { get; }

        Task PrintAsync(Guid? templateKey);

        Task PrintDirectAsync(Guid? templateKey);

        Task ShowPreviewAsync(Guid? templateKey);

        Task ExportAsync(Guid? templateKey, ExportFormat format, string defaultFileName = null);

        Task ExportAsync(IRichReportTemplate template, ExportFormat format, string defaultFileName = null);

        Task PrintAsync(IRichReportTemplate template);

        Task ShowPrintDialogAsync(IRichReportTemplate template);

        Task ShowPrintPreviewAsync(IRichReportTemplate template);

        internal void OnCalculatedFieldChanged(string fieldName);

        internal void OnCalculatedFieldRemoved(string fieldName);
    }
}
