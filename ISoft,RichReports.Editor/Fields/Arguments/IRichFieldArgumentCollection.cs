using System.Collections.Generic;
using System.Text;

namespace ISoft.RichReports.Editor.Fields.Arguments
{
    public interface IRichFieldArgumentCollection : IEnumerable<IRichFieldArgument>
    {
        IRichFieldArgument this[int index] { get; set; }

        int Count { get; }
        int MaxIndex { get; }
        int MinIndex { get; }

        internal void Add(IRichFieldArgument item);

        void AddOrUpdateValue(int index, string value);

        void AddValue(string value);

        void AddValueAtIndex(int index, string value);

        void BuildTo(StringBuilder sb);

        string GetBuildString();

        internal void Clear();

        bool Contains(IRichFieldArgument item);

        bool ContainsAt(int index);

        void CopyTo(IRichFieldArgument[] array, int arrayIndex);

        IRichFieldArgument FirstOrDefault();

        IRichFieldArgument LastOrDefault();

        string GetFirstValue(string defaultValue = null);

        IEnumerable<int> GetIndices();

        IEnumerable<IRichFieldArgument> GetOrderedArguments();

        string GetStringValue(int index, string defaultValue = null);

        T GetValue<T>(int index, T defaultValue = default);

        bool TryGetValueOrDefault<T>(int index, T defaultValue, out T value);

        IEnumerable<string> GetValues();

        void InsertValue(int index, string value);

        void UpdateValue(int index, string value);

        bool Remove(IRichFieldArgument item);

        bool RemoveValue(int index);
        bool TryGetValue<T>(int index, out T value);
    }
}



