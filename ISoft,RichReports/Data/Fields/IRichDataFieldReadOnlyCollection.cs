using System.Collections.Generic;

namespace ISoft.RichReports.Data.Fields
{
    public interface IRichDataFieldReadOnlyCollection : IReadOnlyCollection<IRichDataField>
    {
        IRichDataField this[string path] { get; }

        IEnumerable<string> Paths { get; }

        bool ContainsField(string path);

        IRichDataField GetFieldByPath(string path);

        string GetFieldPath(IRichDataField item);

        bool TryGetField(string path, out IRichDataField item);
    }

}
