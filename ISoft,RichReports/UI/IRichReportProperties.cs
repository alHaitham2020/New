namespace ISoft.RichReports.UI
{
    public interface IRichReportProperties
    {
        string DatabaseName { get; }

        string FileName { get; }

        string ServerName { get; }

        string UserName { get; }

        string AppVersion {  get; }
    }
}
