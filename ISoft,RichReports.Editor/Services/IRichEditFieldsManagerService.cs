using ISoft.RichReports.Editor.Fields.Core;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Services
{
    public interface IRichEditFieldsManagerService
    {
        IReadOnlyDictionary<RichEditFieldType, RichEditFieldInfo> GetAllFieldInfos();

        IEnumerable<string> GetAllFieldNames();

        string GetFieldDisplayName(RichEditFieldType fieldType);

        RichEditFieldInfo? GetFieldInfo(RichEditFieldType fieldType);

        RichEditFieldInfo? GetFieldInfoByName(string fieldName);

        IEnumerable<RichEditFieldInfo> GetFieldsByType(Type type);

        RichEditFieldType? GetFieldTypeByName(string fieldName);

        bool HasFieldName(string fieldName);

        bool HasFieldType(RichEditFieldType fieldType);

        bool TryGetFieldInfo(RichEditFieldType fieldType, out RichEditFieldInfo info);

        bool TryGetFieldInfoByName(string fieldName, out RichEditFieldInfo info);
    }
}
