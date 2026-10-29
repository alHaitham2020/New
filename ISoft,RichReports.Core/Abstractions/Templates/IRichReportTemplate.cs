using System;

namespace ISoft.RichReports.Core.Templates
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
