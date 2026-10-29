using DevExpress.Data;

namespace ISoft.RichReports.Editor.Data
{
    /// <summary>
    /// A column paired with its dotted path from the root.
    /// </summary>
    public readonly struct RichDataColumnWithPath
    {
        public IDataColumnInfo Column { get; }

        public string Path { get; }

        public RichDataColumnWithPath(IDataColumnInfo column, string path)
        {
            Column = column;
            Path = path;
        }

        public void Deconstruct(out IDataColumnInfo column, out string path)
        {
            column = Column;
            path = Path;
        }
    }
}
