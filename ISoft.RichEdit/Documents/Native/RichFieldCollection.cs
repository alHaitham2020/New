using System.Collections.Generic;

namespace ISoft.RichEdit.Documents.Native
{
    public interface IRichDocField
    {
        SubDocumentType DocumentType { get; }

        Field Field { get; }
    }

    public interface IRichFieldCollection : IReadOnlyDataCollection<IRichDocField>
    {

    }

    public class RichFieldCollection : List<IRichDocField>, IRichFieldCollection
    {
        public RichFieldCollection()
        {

        }

        public RichFieldCollection(IEnumerable<IRichDocField> collection) : base(collection)
        {

        }

        public RichFieldCollection(int capacity) : base(capacity)
        {
        }
    }
}
