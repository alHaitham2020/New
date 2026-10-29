namespace ISoft.RichReports.Editor.Fields.Arguments
{
    /// <summary>
    /// Represents a single field argument
    /// </summary>
    public interface IRichFieldArgument
    {
        /// <summary>
        /// The index of this argument in the collection (0-based)
        /// </summary>
        int Index { get; }

        /// <summary>
        /// The raw value of the argument
        /// </summary>
        string Value { get; }

        T GetValue<T>(T defaultValue = default);

        string Build();

        bool IsValid();

        bool TryBuild(out string result);

        internal void UpdateIndex(int index);

        /// <summary>
        /// Creates a copy of the argument
        /// </summary>
        IRichFieldArgument Copy();
    }
}

   

