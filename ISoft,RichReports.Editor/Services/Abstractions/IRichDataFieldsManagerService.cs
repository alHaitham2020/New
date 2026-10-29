using ISoft.RichReports.Editor.Data;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichDataFieldsManagerService
    {
        string GeFieldActualDataName(string fieldName);

        string GetFieldDisplayName(string fieldName);

        object GetFieldDisplayValue(string fieldName, object fieldValue);

        IRichFieldDataInfo GetFieldInfo(string fieldName);
    }
}