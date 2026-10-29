using System.ComponentModel.Design;

namespace ISoft.RichReports.Core.Model
{
    public interface IRichServiceContainer
    {
        void CopyRequiredServices(IServiceContainer targetContainer);

        void CopyRequiredPreviewServices(IServiceContainer targetContainer);

        void CopyRequiredDesignServices(IServiceContainer targetContainer);

        T GetService<T>() where T : class;

        T ReplaceService<T>(T newService) where T : class;

        bool TryGetService<T>(out T service) where T : class;

        bool HasService<T>() where T : class;
       
    }
}
