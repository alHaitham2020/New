using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditDocumentRenderingService
    {
        void Render(Document doc);

        Task RenderAsync(Document doc);
    }
}
