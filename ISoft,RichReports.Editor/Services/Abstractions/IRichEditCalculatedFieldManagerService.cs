using ISoft.RichReports.Editor.RichProperties;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditCalculatedFieldManagerService
    {
        IRichCalculatedFieldProperties CalculatedFieldProperties { get; }

        string GetFieldDisplayName(string fieldName);

        string GetFieldExpression(string fieldName);

        bool TryGetFieldExpression(string fieldName, out string expression);
    }
}
