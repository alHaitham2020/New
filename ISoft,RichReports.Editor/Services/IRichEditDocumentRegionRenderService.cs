using DevExpress.XtraRichEdit.API.Native;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditDocumentRegionRenderService
    {
        void Render(Document document);

        Task RenderAsync(Document document, CancellationToken cancellationToken = default);
    }
}
