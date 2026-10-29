namespace ISoft.RichReports.Editor.Preview
{
    public interface IRichDocumentPreviewOptions
    {
        object DataSource { get; }

        DocumentFormat DocumentFormat { get; }

        int FirstRecordIndex { get; }

        int LastRecordIndex { get; }

        string PreviewTitle { get; }

        IRichReportTemplate Template { get; }
    }

    public class RichDocumentPreviewOptions : IRichDocumentPreviewOptions
    {
        public DocumentFormat DocumentFormat { get; set; }

        public IRichReportTemplate Template { get; set; }

        public object DataSource { get; set; }

        public int FirstRecordIndex { get; set; } = 0;

        public int LastRecordIndex { get; set; } = -1;

        public string PreviewTitle { get; set; }
    }
}
