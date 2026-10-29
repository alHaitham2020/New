namespace ISoft.RichReports.Fields
{
    public interface IRichFieldGroup : IRichNode
    {
        IRichFieldGroup ParentGroup { get;}

        RichFieldGroupCategory Category { get;}
    }
}
