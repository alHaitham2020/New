using System;

namespace ISoft.RichReports.Fields
{
    public abstract class RichFieldGroupBase : RichNodeBase, IRichFieldGroup
    {
        public override bool IsGroup => true;

        public RichFieldGroupCategory Category { get; set; }

        public RichFieldGroupBase(
            string name,
            string displayName,
            string path,
            RichFieldGroupCategory category,
            Type dataType = null,
            IRichFieldGroup parentGroup = null,
            bool? isVisible = null)
        {
            Name = name;
            DisplayName = displayName;
            Path = path;
            Category = category;
            DataType = dataType;
            ParentGroup = parentGroup;
            IsVisible = isVisible ?? true;
        }
    }
}
