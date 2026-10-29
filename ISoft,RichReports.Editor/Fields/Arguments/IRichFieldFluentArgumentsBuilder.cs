namespace ISoft.RichReports.Editor.Fields.Argumentes
{
    public interface IRichFieldFluentArgumentsBuilder
    {
        IRichFieldFluentArgumentsBuilder Add(string value);
        IRichFieldFluentArgumentsBuilder AddOrUpdate(int index, string value);
        IRichFieldFluentArgumentsBuilder AddValueAt(int index, string value);
        IRichFieldFluentArgumentsBuilder InsertValueAt(int index, string value);
        IRichFieldFluentArgumentsBuilder RemoveValueAt(int index);
        string BuildString();
    }
}

   

