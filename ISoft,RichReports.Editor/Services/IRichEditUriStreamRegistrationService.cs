using DevExpress.Office.Services;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditUriStreamRegistrationService
    {
        void RegisterUriStreamProviders(IUriStreamService uriStreamService);

        void UnRegisterUriStreamProviders(IUriStreamService uriStreamService);

    }
}
