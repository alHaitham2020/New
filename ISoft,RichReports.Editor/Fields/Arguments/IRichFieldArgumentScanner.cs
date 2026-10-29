using System.Collections.Generic;

namespace ISoft_RichReports.Editor.Fields.Arguments
{
    public interface IRichFieldArgumentScanner
    {
        string AppendArgument(string parameters, string newValue, bool appendAtEnd = true);

        int CountArguments(string parameters, bool permissiveMode = false);

        string ExtractArgument(string parameters, int index, bool permissiveMode = false);

        List<string> ExtractArguments(string parameters, bool permissiveMode = false);

        List<ArgumentPosition> ExtractArgumentsWithPositions(string parameters);

        string ExtractFirstArgument(string parameters, bool permissiveMode = false);

        string FormatArgument(string value);

        bool IsValidArgument(string text);

        string RemoveArgument(string parameters, int index);

        string RemoveArguments(string parameters);

        string ReplaceArgument(string parameters, int index, string newValue);

        string ReplaceFirstArgument(string parameters, string newValue);
    }
}
