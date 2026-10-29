using System.Collections.Generic;

namespace ISoft.RichReports.Designer.Fields
{
    public interface IRichDisplayFieldsReadOnlyCollection : IReadOnlyCollection<IRichDisplayField>
    {
        IRichDisplayField this[RichEditFieldKey key] { get; }

        IEnumerable<RichEditFieldKey> FieldKeys { get; }

        bool ContainsField(RichEditFieldKey key);

        IRichDisplayField FindByFieldProperty(string fieldProperty);

        IRichDisplayField GetFieldByKey(RichEditFieldKey key);

        bool TryGetField(RichEditFieldKey key, out IRichDisplayField item);
    }

}
