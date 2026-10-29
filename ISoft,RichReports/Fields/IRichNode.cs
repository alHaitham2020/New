using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Fields
{
    public interface IRichNode : ICloneable
    {
        string Name { get;}

        string DisplayName { get;}

        string Path { get;}

        Type DataType { get;}

        bool IsVisible { get;}

        bool IsGroup { get; }

        Type GetActualDataType();

        IEnumerable<IRichNode> GetAncestors();

        bool IsDescendantOf(IRichNode node);
    }
}
