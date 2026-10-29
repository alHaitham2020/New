namespace ISoft.RichReports.Editor.Fields.Argumentes
{
    public interface IRichFieldArgumentBuilder
    {
        IRichFieldArgumentBuilder Add(object value);

        string Build();
    }
}

   

