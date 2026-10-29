namespace ISoft.RichReports.Editor.Fields.Arguments
{
    public interface IRichFieldArgumentBuilder
    {
        IRichFieldArgumentBuilder Add(object value);

        string Build();
    }
}

   

