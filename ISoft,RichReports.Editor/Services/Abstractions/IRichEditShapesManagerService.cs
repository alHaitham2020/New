namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditShapesManagerService
    {
        Shape InsertShape(DocumentPosition position, ShapeGeometryPreset preset);
    }
}
