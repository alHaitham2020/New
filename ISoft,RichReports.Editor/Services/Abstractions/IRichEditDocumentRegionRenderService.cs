using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditDocumentRegionRenderService
    {
        void Render(Document document);

        Task RenderAsync(Document document, CancellationToken cancellationToken = default);
    }
}
