using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Fields.Codes
{
    public interface IRichFieldCodeCollection : IReadOnlyDataCollection<IRichFieldCode>
    {

    }

    public class RichFieldCodeCollection : List<IRichFieldCode>, IRichFieldCodeCollection
    {
        public RichFieldCodeCollection()
        {

        }

        public RichFieldCodeCollection(IEnumerable<IRichFieldCode> collection) : base(collection)
        {

        }

        public RichFieldCodeCollection(int capacity) : base(capacity)
        {
        }
    }
}
