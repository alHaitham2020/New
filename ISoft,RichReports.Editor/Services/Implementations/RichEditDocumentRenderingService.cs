using DevExpress.XtraRichEdit.API.Native;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services.Implementation
{
    public class RichEditDocumentRenderingService : IRichEditDocumentRenderingService
    {
        private readonly IRichEditDocumentRegionRenderService _renderService;

        public RichEditDocumentRenderingService(IRichEditDocumentRegionRenderService tableFieldsRenderService)
        {
            _renderService = tableFieldsRenderService ?? throw new System.ArgumentNullException(nameof(tableFieldsRenderService));
        }

        public RichEditDocumentRenderingService(IRichEditDocumentFieldManagerService fieldManagerService) 
            : this(new RichEditDocumentRegionRenderService(fieldManagerService))
        {

        }

        public void Render(Document doc)
        {
            _renderService.Render(doc);
        }

        public async Task RenderAsync(Document doc)
        {
           await _renderService.RenderAsync(doc);
        }
    }
}
