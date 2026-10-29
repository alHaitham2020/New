using DevExpress.XtraRichEdit.API.Native;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditShapesManagerService
    {
        Shape InsertShape(DocumentPosition position, ShapeGeometryPreset preset);
    }
}
