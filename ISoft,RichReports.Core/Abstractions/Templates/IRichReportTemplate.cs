using ISoft.RichReports.Core.Models.Templates;
using System;

namespace ISoft.RichReports.Core.Abstractions.Templates
{
    public interface IRichReportTemplate : IRichReportTemplateData
    {
        internal void UpdateContent(byte[] data);

        internal void UpdateThumbnail(byte[] data);

        string GetDesignerTitle();

        string GetDisplayName();

        string GetPreviewTitle();

        IRichReportTemplate CreateCopy(Guid id);

        IRichReportTemplate CreateCopy(Guid id,string name);

        RichReportTemplateData CreateTemplateData();
    }
}
