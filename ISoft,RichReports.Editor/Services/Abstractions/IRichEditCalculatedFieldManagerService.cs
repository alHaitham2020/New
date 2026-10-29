using ISoft.RichReports.Editor.Properties.RichProperties;

namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichEditCalculatedFieldManagerService
    {
        IRichCalculatedFieldProperties CalculatedFieldProperties { get; }

        string GetFieldDisplayName(string fieldName);

        string GetFieldExpression(string fieldName);

        bool TryGetFieldExpression(string fieldName, out string expression);
    }
}
