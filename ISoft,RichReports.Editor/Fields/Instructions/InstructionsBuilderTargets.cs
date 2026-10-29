using System;

namespace ISoft.RichReports.Editor.Fields.Instructions
{
    [Flags]
    public enum InstructionsBuilderTargets
    {
        Arguments = 1,
        Switches = 2,
        All = Arguments | Switches
    }
}
