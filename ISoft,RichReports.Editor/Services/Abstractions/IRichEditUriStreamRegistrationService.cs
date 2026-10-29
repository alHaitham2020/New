namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditUriStreamRegistrationService
    {
        void RegisterUriStreamProviders(IUriStreamService uriStreamService);

        void UnRegisterUriStreamProviders(IUriStreamService uriStreamService);

    }
}
