using DevExpress.Office.Services;
using ISoft.Office.Images;

namespace ISoft.RichReports.Editor.Services.Implementation
{
    public class RichEditUriStreamProviderManagerService : IRichEditUriStreamRegistrationService
    {
        public RichEditUriStreamProviderManagerService()
        {

        }

        public virtual void RegisterUriStreamProviders(IUriStreamService uriStreamService)
        {
            //uriStreamService.RegisterProvider(new Base64ImageProvider());
            //uriStreamService.RegisterProvider(new FileSystemImageProvider());
            //uriStreamService.RegisterProvider(new ResourceImageProvider());

            uriStreamService.RegisterProvider(ImageUriStreamProvidersManager.CreateDefault());
        }

        public virtual void UnRegisterUriStreamProviders(IUriStreamService uriStreamService)
        {

        }
    }
}
