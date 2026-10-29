namespace ISoft.RichReports.Editor.RichProperties
{
    public interface IRichCalculatedFieldProperty : IRichProperty
    {
        string DataMember { get; }

        string Expression { get; }

        bool HasValidDataMember { get; }
    }
}
