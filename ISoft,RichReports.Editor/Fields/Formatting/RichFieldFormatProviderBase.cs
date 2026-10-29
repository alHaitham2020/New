using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Editor.Fields.Formatting
{
    public abstract class RichFieldFormatProviderBase<TFormatId> : IRichFieldFormatProvider<TFormatId> where TFormatId : struct
    {
        protected readonly ConcurrentDictionary<TFormatId, RichFieldFormat> Formats;
        protected readonly ConcurrentDictionary<Type, TFormatId> DefaultFormats;
        private readonly object _lock = new object();

        public abstract RichFieldFormatType FormatType { get; }

        public RichFieldFormatProviderBase()
        {
            Formats = new ConcurrentDictionary<TFormatId, RichFieldFormat>();
            DefaultFormats = new ConcurrentDictionary<Type, TFormatId>();

            PopulateFormats();
            PopulateDefaultFormats();
        }

        #region IRichFieldFormatProvider Implementation
        public virtual void Register(TFormatId type, string value, string displayName)
        {
            Register(type, new RichFieldFormat(FormatType, type.ToString(), value, displayName));
        }

        public virtual void Register(TFormatId type, RichFieldFormat format)
        {
            if (format.FormatType != FormatType)
                throw new ArgumentException($"Format type mismatch. Expected {FormatType}, got {format.FormatType}");

            lock (_lock)
            {
                Formats[type] = format;
            }
        }

        public virtual void Unregister(TFormatId type)
        {
            lock (_lock)
            {
                if (Formats.ContainsKey(type))
                    Formats.TryRemove(type, out _);
            }
        }

        public virtual RichFieldFormat[] GetAllFormats() => Formats.Values.ToArray();

        public virtual bool HasFormat(TFormatId formatType)
        {
            lock (_lock)
            {
                return Formats.ContainsKey(formatType);
            }
        }

        public virtual bool HasDefaultFormatValue(Type dataType)
        {
            if (dataType == null)
                return false;

            lock (_lock)
            {
               return DefaultFormats.ContainsKey(dataType);
            }
        }

        public virtual string GetDefaultFormatValue(Type dataType)
        {
            if (dataType == null)
                return null;

            lock (_lock)
            {
                if (DefaultFormats.TryGetValue(dataType, out var formatName) && Formats.TryGetValue(formatName, out var format))
                {
                    return format.Value;
                }
            }

            return null;
        }

        public virtual RichFieldFormat? GetDefaultFormat(Type dataType)
        {
            if (dataType == null)
                return null;

            lock (_lock)
            {
                if (DefaultFormats.TryGetValue(dataType, out var formatName) && Formats.TryGetValue(formatName, out var format))
                {
                    return format;
                }
            }

            return null;
        }

        public virtual bool TryGetDefaultFormat(Type dataType, out RichFieldFormat? result)
        {
            if (dataType != null && DefaultFormats.TryGetValue(dataType, out var formatName) && Formats.TryGetValue(formatName, out var format))
            {
                result = format;
                return true;
            }

            result = null;
            return false;
        }

        public virtual bool TryGetDefaultFormatValue(Type dataType, out string result)
        {
            if (dataType != null && DefaultFormats.TryGetValue(dataType, out var formatName) &&Formats.TryGetValue(formatName, out var format))
            {
                result = format.Value;
                return true;
            }

            result = null;
            return false;
        }

        public virtual RichFieldFormat? GetFormat(TFormatId type)
        {
            lock (_lock)
            {
                Formats.TryGetValue(type, out var format);
                return format;
            }
        }

        public virtual bool TryGetFormat(TFormatId type, out RichFieldFormat result)
        {
            lock (_lock)
            {
                return Formats.TryGetValue(type, out result);
            }
        }

        public virtual RichFieldFormat? GetFormatByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            lock (_lock)
            {
                if (EnumerationHelper.TryParse<TFormatId>(name, out var id) && Formats.TryGetValue(id, out var format))
                    return format;

                return null;
            }
        }

        public virtual bool TryGetFormatByName(string name, out RichFieldFormat result)
        {
            if (string.IsNullOrEmpty(name))
            {
                result = RichFieldFormat.Empty;
                return false;
            }

            lock (_lock)
            {
                if (EnumerationHelper.TryParse<TFormatId>(name, out var id))
                    return Formats.TryGetValue(id, out result);

                result = RichFieldFormat.Empty;
                return false;
            }
        }

        public RichFieldFormatType? GetFormatTypeForValue(string formatValue)
        {
            if (string.IsNullOrEmpty(formatValue)) return null;

            foreach (var format in Formats.Values)
            {
                if (format.IsValid() && format.Value.Equals(formatValue, StringComparison.OrdinalIgnoreCase))
                    return format.FormatType;
            }

            return null;
        }

        public RichFieldFormat? GetFormatForValue(string formatValue)
            => TryGetFormatForValue(formatValue, out var format) ? format : null;

        public bool TryGetFormatForValue(string formatValue, out RichFieldFormat resultFormat)
        {
            resultFormat = RichFieldFormat.Empty;

            if (string.IsNullOrEmpty(formatValue))
                return false;

            foreach (var format in Formats.Values)
            {
                if (IsMatch(format, formatValue))
                {
                    resultFormat = format;
                    return true;
                }
            }

            return false;
        }

        private static bool IsMatch(RichFieldFormat format, string value)
            => format.IsValid()
               && string.Equals(format.Value, value, StringComparison.OrdinalIgnoreCase);


        public virtual List<string> GetAvailableFormatValues(Type dataType)
        {
            var formats = GetAvailableFormats(dataType);
            return formats.Select(f => f.Value).Where(v => !string.IsNullOrEmpty(v)).ToList();
        }

        public virtual List<RichFieldFormat> GetAvailableFormats(Type dataType)
        {
            if (dataType == null)
                return new List<RichFieldFormat>();

            return GetAvailableFormatsInternal(dataType);
        }

        #endregion

        #region Abstarct
        protected virtual void RegisterDefaultFormat(Type dataType, TFormatId formatTypeId)
        {
            DefaultFormats[dataType] = formatTypeId;
        }

        protected abstract List<RichFieldFormat> GetAvailableFormatsInternal(Type dataType);

        protected abstract void PopulateFormats();

        protected abstract void PopulateDefaultFormats();
        #endregion
    }
}
