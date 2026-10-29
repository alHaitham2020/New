namespace ISoft.RichReports.Editor.Services
{
    public interface IRichProgressIndicationService
    {
        void Begin(string displayName, int minProgress, int maxProgress, int currentProgress);

        void SetProgress(int currentProgress);

        void End();

        void SuspendProgressIndication();

        void ResumeProgressIndication();
    }
}
