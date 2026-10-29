namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichMailMergeClientService
    {
        void Started(MailMergeStartedEventArgs e);

        void Finished(MailMergeFinishedEventArgs e);

        void RecordStarted(MailMergeRecordStartedEventArgs e);

        void RecordFinished(MailMergeRecordFinishedEventArgs e);
    }
}
