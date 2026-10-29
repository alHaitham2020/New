using ISoft.RichReports.Fields;
using System.Collections.Generic;

namespace ISoft.RichReports.Designer.Fields
{
    public interface IRichDisplayFieldGroupReadOnlyCollection : IReadOnlyCollection<IRichDisplayFieldGroup>
    {
        IEnumerable<string> GroupPaths { get; }

        bool ContainsGroup(string path);

        bool TryGetGroup(string path, out IRichDisplayFieldGroup item);

        IRichDisplayField FindFieldByPath(string path);

        IRichDisplayField FindFieldByKey(RichEditFieldKey fieldKey);

        IRichDisplayField FindNoneDataFieldByKey(RichEditFieldKey fieldKey);

        IRichDisplayFieldGroup FindGroup(string path);

        IRichDisplayField FindDataFieldByPath(string fieldPath);

        IRichDisplayFieldGroup FindGroupContainingField(RichEditFieldKey fieldKey);

        IRichDisplayField FindDataFieldByKey(RichEditFieldKey fieldKey);

        IRichDisplayFieldGroup FindGroupByCategory(RichFieldGroupCategory category);

        IRichDisplayFieldGroupReadOnlyCollection GetGroupsIn(params RichFieldGroupCategory[] categories);

        IRichDisplayFieldGroupReadOnlyCollection GetGroupsNotIn(params RichFieldGroupCategory[] categories);

        IReadOnlyCollection<IRichDisplayField> GetAllFields();

        IRichDisplayFieldGroupReadOnlyCollection GetNoneDataFieldGroups();

        IRichDisplayFieldGroupReadOnlyCollection GetDataFieldGroups();

        IReadOnlyCollection<IRichDisplayField> GetNoneDataFields();

    }
}
