namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    // provide a localized display name for propeyy
    public interface IRichDocumentPropertyDisplayNameProvider
    {
        string GetDisplayName(RichDocumentPropertyId id);
    }
}
