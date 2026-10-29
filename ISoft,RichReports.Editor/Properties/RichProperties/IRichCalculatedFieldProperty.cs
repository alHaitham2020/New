namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    public interface IRichCalculatedFieldProperty : IRichProperty
    {
        string DataMember { get; }

        string Expression { get; }

        bool HasValidDataMember { get; }
    }
}
