using DevExpress.XtraRichEdit.API.Native;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditDocumentRenderingService
    {
        void Render(Document doc);

        Task RenderAsync(Document doc);
    }
}
