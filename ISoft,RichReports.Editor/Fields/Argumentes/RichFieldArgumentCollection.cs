using ISoft.Utils.Extentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISoft.RichReports.Editor.Fields.Argumentes
{
    /// <summary>
    /// Implementation of field argument collection using an index-based dictionary
    /// for O(1) lookups and efficient operations.
    /// </summary>
    public class RichFieldArgumentCollection : IRichFieldArgumentCollection
    {
        private readonly Dictionary<int, IRichFieldArgument> _arguments;
        private int _nextIndex;

        public RichFieldArgumentCollection()
        {
            _arguments = new Dictionary<int, IRichFieldArgument>();
            _nextIndex = 0;
        }

        public RichFieldArgumentCollection(IEnumerable<string> values)
        {
            var valueList = values?.ToList() ?? new List<string>();
            _arguments = new Dictionary<int, IRichFieldArgument>(valueList.Count);
            _nextIndex = 0;

            foreach (var value in valueList)
            {
                AddValue(value);
            }
        }

        public RichFieldArgumentCollection(IEnumerable<IRichFieldArgument> arguments)
        {
            var argList = arguments?.ToList() ?? new List<IRichFieldArgument>();
            _arguments = new Dictionary<int, IRichFieldArgument>(argList.Count);
            _nextIndex = 0;

            foreach (var argument in argList)
            {
                Add(argument);
            }
        }

        #region IRichFieldArgumentCollection Implementation

        public IRichFieldArgument this[int index]
        {
            // Returns null safely if index isn't found
            get => _arguments.TryGetValue(index, out var arg) ? arg : null;
            set
            {
                if (value == null)
                {
                    if (_arguments.Remove(index) && index == _nextIndex - 1) _nextIndex--;
                    return;
                }

                value.UpdateIndex(index);

                _arguments[index] = value;
                UpdateNexIndex(index + 1);
            }
        }

        public int Count => _arguments.Count;

        public int MaxIndex => _arguments.Count > 0 ? _arguments.Keys.Max() : -1;

        public int MinIndex => _arguments.Count > 0 ? _arguments.Keys.Min() : -1;

        public IRichFieldArgument FirstOrDefault()
        {
            return _arguments.Count > 0 ? _arguments[_arguments.Keys.First()] : null;
        }

        public IRichFieldArgument LastOrDefault()
        {
            return _arguments.Count > 0 ? _arguments[_arguments.Keys.Last()] : null;
        }

        /// <summary>Gets first argument as string, or default if empty.</summary>
        public string GetFirstValue(string defaultValue = null)
        {
            return Count > 0 ? GetStringValue(0, defaultValue) : defaultValue;
        }

        public void Add(IRichFieldArgument item)
        {
            if (item == null) return;

            // If item already has a valid unique index, preserve it
            if (item.Index >= 0 && !_arguments.ContainsKey(item.Index))
            {
                _arguments[item.Index] = item;
                UpdateNexIndex(item.Index + 1);
            }
            else
            {
                // Assign next available index safely if the object type allows it
                item.UpdateIndex(_nextIndex);
                _arguments[_nextIndex] = item;
                _nextIndex++;
            }
        }

        public void AddValue(string value)
        {
            _arguments[_nextIndex] = new RichFieldArgument(_nextIndex, value);
            _nextIndex++;
        }

        /// <summary>
        /// Adds a new element only if the index does not already exist. 
        /// Throws an exception if the index is taken.
        /// </summary>
        public void AddValueAtIndex(int index, string value)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");
            if (_arguments.ContainsKey(index)) throw new ArgumentException($"An item with index {index} already exists.", nameof(index));

            _arguments.Add(index, new RichFieldArgument(index, value));
            UpdateNexIndex(index + 1);
        }

        /// <summary>
        /// Overwrites the element if the index exists, otherwise adds a brand new element.
        /// </summary>
        public void AddOrUpdateValue(int index, string value)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

            _arguments[index] = new RichFieldArgument(index, value);
            UpdateNexIndex(index + 1);
        }

        /// <summary>
        /// Overwrites the element if the index exists, otherwise Throws an exception if the index is not found
        /// </summary>
        public void UpdateValue(int index, string value)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

            if (!_arguments.ContainsKey(index)) throw new ArgumentException($"An item with index {index} not exists.", nameof(index));

            _arguments[index] = new RichFieldArgument(index, value);
            UpdateNexIndex(index + 1);
        }

        public void InsertValue(int index, string value)
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index cannot be negative.");

            if (!_arguments.ContainsKey(index))
            {
                AddOrUpdateValue(index, value);
                return;
            }

            var keysToShift = _arguments.Keys.Where(k => k >= index).OrderByDescending(k => k).ToList();

            foreach (var key in keysToShift)
            {
                var item = _arguments[key];
                _arguments.Remove(key);

                item.UpdateIndex(key + 1);
                _arguments[key + 1] = item;
            }

            _arguments[index] = new RichFieldArgument(index, value);
            UpdateNexIndex(_arguments.Keys.MaxOrDefault() + 1);
        }

        public void Clear()
        {
            _arguments.Clear();
            _nextIndex = 0;
        }

        public bool Contains(IRichFieldArgument item)
            => item != null && _arguments.TryGetValue(item.Index, out var existing) && existing == item;

        public bool ContainsAt(int index)
        {
            return _arguments.ContainsKey(index);
        }

        public bool Remove(IRichFieldArgument item)
            => item != null && RemoveValue(item.Index);

        public bool RemoveValue(int index)
        {
            if (!_arguments.Remove(index))
                return false;

            if (index == _nextIndex - 1)
            {
                _nextIndex--;
            }

            return true;
        }

        /// <summary>Gets typed value at index. Returns default if out of range or conversion fails.</summary>
        public T GetValue<T>(int index, T defaultValue = default)
        {
            if (index < 0 || index >= Count)
                return defaultValue;

            try
            {
                var arg = this[index];
                if (arg == null) return defaultValue;

                return arg.GetValue(defaultValue);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>Gets string value at index. Returns default if out of range.</summary>
        public string GetStringValue(int index, string defaultValue = null)
        {
            if (index < 0 || index >= Count)
                return defaultValue;

            return GetValue(index, defaultValue);
        }

        /// <summary>Attempts to get typed value at index. Returns false if out of range or conversion fails.</summary>
        public bool TryGetValueOrDefault<T>(int index, T defaultValue, out T value)
        {
            value = defaultValue;

            if (index < 0 || index >= Count)
                return false;

            try
            {
                if (_arguments.TryGetValue(index, out var arg) && arg.IsValid())
                {
                    value = arg.GetValue(defaultValue);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>Attempts to get typed value at index. Returns false if out of range or conversion fails.</summary>
        public bool TryGetValue<T>(int index, out T value)
        {
            return TryGetValueOrDefault(index, default(T), out value);
        }

        /// <summary>Gets all argument values as strings.</summary>
        public IEnumerable<string> GetValues()
        {
            return _arguments.Select(a => a.Value.Value);
        }

        public IEnumerable<int> GetIndices()
        {
            return _arguments.Keys.ToList();
        }

        public void CopyTo(IRichFieldArgument[] array, int arrayIndex)
        {
            _arguments.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets arguments sorted by index
        /// </summary>
        public IEnumerable<IRichFieldArgument> GetOrderedArguments()
        {
            return _arguments.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
        }

        #endregion

        #region IEnumerator Implementation

        public IEnumerator<IRichFieldArgument> GetEnumerator() => _arguments.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
        
        #region Bulk Operations & Output
        /// <summary>
        /// Rebuilds indices to be sequential starting from 0
        /// </summary>
        internal void RebuildIndices()
        {
            var orderedArgs = GetOrderedArguments().ToList();
            _arguments.Clear();
            _nextIndex = 0;

            foreach (var arg in orderedArgs)
            {
                //var richArgument = arg as RichFieldArgument;
                //if (richArgument != null)
                //{
                //    richArgument.Index = _nextIndex;
                //}
                arg.UpdateIndex(_nextIndex);
                _arguments[_nextIndex] = arg;
                _nextIndex++;
            }
        }

        /// <summary>Gets string of all arguments, each wrapped in quotes.</summary>
        public string GetBuildString()
        {
            var sb = new StringBuilder();

            BuildTo(sb);

            return sb.ToString();
        }

        /// <summary>Appends quoted arguments to StringBuilder. More efficient for chaining.</summary>
        public void BuildTo(StringBuilder sb)
        {
            var coll = GetOrderedArguments().ToList();
            foreach (var kv in coll)
            {
                if (kv.TryBuild(out var value))
                    sb.Append(value);
            }
        }

        #endregion

        #region Core
        private void UpdateNexIndex(int index)
        {
            _nextIndex = Math.Max(_nextIndex, index);
        }

        #endregion

    }
}
