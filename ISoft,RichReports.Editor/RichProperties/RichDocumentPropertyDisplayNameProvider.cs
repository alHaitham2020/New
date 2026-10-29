using System.Collections.Generic;

namespace ISoft.RichReports.Editor.RichProperties
{
    /// <summary>
    /// default provider with English and Arabic localization support
    /// </summary>
    public class RichDocumentPropertyDisplayNameProvider : IRichDocumentPropertyDisplayNameProvider
    {
        private readonly Dictionary<RichDocumentPropertyId, string> _displayNames;

        public RichDocumentPropertyDisplayNameProvider(string culture = "ar-sa")
        {
            _displayNames = new Dictionary<RichDocumentPropertyId, string>();
            LoadLocalizedNames(culture);
        }

        protected virtual void LoadLocalizedNames(string culture)
        {
            switch (culture.ToLower())
            {
                case "ar-sa":
                case "ar-eg":
                case "ar":
                    // Arabic names
                    SetDisplayName(RichDocumentPropertyId.Category, "الفئة");
                    SetDisplayName(RichDocumentPropertyId.ContentStatus, "حالة المحتوى");
                    SetDisplayName(RichDocumentPropertyId.ContentType, "نوع المحتوى");
                    SetDisplayName(RichDocumentPropertyId.Created, "تاريخ الإنشاء");
                    SetDisplayName(RichDocumentPropertyId.Author, "المؤلف");
                    SetDisplayName(RichDocumentPropertyId.Creator, "المنشئ");
                    SetDisplayName(RichDocumentPropertyId.Description, "الوصف");
                    SetDisplayName(RichDocumentPropertyId.Identifier, "المعرف");
                    SetDisplayName(RichDocumentPropertyId.Keywords, "الكلمات المفتاحية");
                    SetDisplayName(RichDocumentPropertyId.Language, "اللغة");
                    SetDisplayName(RichDocumentPropertyId.LastModifiedBy, "آخر تعديل بواسطة");
                    SetDisplayName(RichDocumentPropertyId.LastPrinted, "آخر طباعة");
                    SetDisplayName(RichDocumentPropertyId.Modified, "تاريخ التعديل");
                    SetDisplayName(RichDocumentPropertyId.Revision, "رقم المراجعة");
                    SetDisplayName(RichDocumentPropertyId.Subject, "الموضوع");
                    SetDisplayName(RichDocumentPropertyId.Title, "العنوان");
                    SetDisplayName(RichDocumentPropertyId.Version, "الإصدار");
                    SetDisplayName(RichDocumentPropertyId.Application, "التطبيق");
                    SetDisplayName(RichDocumentPropertyId.AppVersion, "إصدار التطبيق");
                    SetDisplayName(RichDocumentPropertyId.Company, "الشركة");
                    SetDisplayName(RichDocumentPropertyId.Manager, "المدير");
                    SetDisplayName(RichDocumentPropertyId.Template, "التصميم");
                    break;

                default:
                    // English names (default)
                    SetDisplayName(RichDocumentPropertyId.Category, "Category");
                    SetDisplayName(RichDocumentPropertyId.ContentStatus, "Content Status");
                    SetDisplayName(RichDocumentPropertyId.ContentType, "Content Type");
                    SetDisplayName(RichDocumentPropertyId.Created, "Created");
                    SetDisplayName(RichDocumentPropertyId.Author, "Author");
                    SetDisplayName(RichDocumentPropertyId.Creator, "Creator");
                    SetDisplayName(RichDocumentPropertyId.Description, "Description");
                    SetDisplayName(RichDocumentPropertyId.Identifier, "Identifier");
                    SetDisplayName(RichDocumentPropertyId.Keywords, "Keywords");
                    SetDisplayName(RichDocumentPropertyId.Language, "Language");
                    SetDisplayName(RichDocumentPropertyId.LastModifiedBy, "Last Modified By");
                    SetDisplayName(RichDocumentPropertyId.LastPrinted, "Last Printed");
                    SetDisplayName(RichDocumentPropertyId.Modified, "Modified");
                    SetDisplayName(RichDocumentPropertyId.Revision, "Revision");
                    SetDisplayName(RichDocumentPropertyId.Subject, "Subject");
                    SetDisplayName(RichDocumentPropertyId.Title, "Title");
                    SetDisplayName(RichDocumentPropertyId.Version, "Version");
                    SetDisplayName(RichDocumentPropertyId.Application, "Application");
                    SetDisplayName(RichDocumentPropertyId.AppVersion, "App Version");
                    SetDisplayName(RichDocumentPropertyId.Company, "Company");
                    SetDisplayName(RichDocumentPropertyId.Manager, "Manager");
                    SetDisplayName(RichDocumentPropertyId.Template, "Template");
                    break;
            }
        }

        public string GetDisplayName(RichDocumentPropertyId id)
        {
            return _displayNames.TryGetValue(id, out var displayName) ? displayName : id.ToString();
        }

        public void SetDisplayName(RichDocumentPropertyId id, string displayName)
        {
            if (!string.IsNullOrEmpty(displayName))
            {
                _displayNames[id] = displayName;
            }
        }

        public Dictionary<RichDocumentPropertyId, string> GetAllDisplayNames()
        {
            return new Dictionary<RichDocumentPropertyId, string>(_displayNames);
        }

        // Method to switch language at runtime
        public void SetLanguage(string culture)
        {
            _displayNames.Clear();
            LoadLocalizedNames(culture);
        }
    }
}
