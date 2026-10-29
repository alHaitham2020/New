using ISoft.RichReports.Editor.Fields.Arguments;
using ISoft.RichReports.Editor.Fields.Codes;
using ISoft.RichReports.Editor.Fields.Switches;

namespace ISoft.RichReports.Editor.Fields.Instructions
{
    public interface IRichFieldInstructionsBuilder
    {
        void BuildInstructions(IRichFieldCode fieldCode, object options, InstructionsBuilderTargets targets);

        void BuildInstructions(IRichFieldCode fieldCode, object options);

        void BuildFieldArguments(IRichFieldArgumentCollection arguments, object options);

        void BuildFieldSwitches(IRichFieldSwitchCollection switches, object options);

        string GetFieldSwitchesBuildString(object options);

        string GetFieldArgumentsBuildString(object options);
    }
}
