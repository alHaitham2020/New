using DevExpress.Mvvm.Native;
using ISoft.RichReports.Editor.Fields.Switches;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace ISoft.RichReports.Editor.Fields.Format
{
    public class RichFieldFormatManager : IRichFieldFormatManager
    {
        #region Fields
        private readonly ConcurrentDictionary<RichFieldFormatType, IRichFieldFormatProvider> _providers;
        private readonly ConcurrentDictionary<string, RichFieldFormat> _formatCache;
        private readonly object _lock = new object();
        #endregion

        #region Singletone
        private static readonly object lockInstance = new object();

        private static readonly Lazy<IRichFieldFormatManager> _instance =
           new(() => new RichFieldFormatManager(),
               LazyThreadSafetyMode.ExecutionAndPublication);

        public static IRichFieldFormatManager Current
        {
            get
            {
                lock (lockInstance)
                {
                    return _instance.Value;
                }
            }
        }
        #endregion

        private RichFieldFormatManager()
        {
            _providers = new ConcurrentDictionary<RichFieldFormatType, IRichFieldFormatProvider>();
            _formatCache = new ConcurrentDictionary<string, RichFieldFormat>(StringComparer.OrdinalIgnoreCase);

            InitializeDefaultProviders();
            BuildCache();
        }

        #region Core
        private void InitializeDefaultProviders()
        {
            RegisterProvider(new RichFieldNumericFormatProvider());
            RegisterProvider(new RichFieldDateTimeFormatProvider());
            RegisterProvider(new RichFieldGeneralFormatProvider());
        }

        private void BuildCache()
        {
            lock (_lock)
            {
                _formatCache.Clear();
                foreach (var provider in _providers.Values)
                {
                    foreach (var format in provider.GetAllFormats())
                    {
                        if (format.IsValid())
                        {
                            _formatCache[format.Value] = format;
                        }
                    }
                }
            }
        }

        // Convenience method to clear all providers and reinitialize
        internal void ResetToDefault()
        {
            lock (_lock)
            {
                _providers.Clear();
                InitializeDefaultProviders();
                BuildCache();
            }
        }
        #endregion

        #region IRichFieldFormatManager Implemntation
        public void RegisterProvider(IRichFieldFormatProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            lock (_lock)
            {
                _providers[provider.FormatType] = provider;
                //BuildCache();
            }
        }

        public IRichFieldFormatProvider GetProvider(RichFieldFormatType formatType)
        {
            if (_providers.TryGetValue(formatType, out var provider))
                return provider;

            throw new ArgumentException($"No provider registered for format type {formatType}", nameof(formatType));
        }

        public bool HasProvider(RichFieldFormatType formatType) => _providers.ContainsKey(formatType);

        public bool HasDefaultFormatValue(Type dataType)
        {
            if (dataType == null)
                return false;

            return _providers.Values.Any(p => p.HasDefaultFormatValue(dataType));
        }

        public RichFieldFormat? GetDefaultFormat(Type dataType)
        {
            if (dataType == null)
                return null;

            foreach (var provider in _providers.Values)
            {
                if (provider.TryGetDefaultFormat(dataType, out var format) && format.HasValue)
                    return format;
            }

            return null;
        }

        public string GetDefaultFormatValue(Type dataType)
        {
            var format = GetDefaultFormat(dataType);
            return format?.Value;
        }

        public RichFieldFormat? GetFormat(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            
            foreach (var provider in _providers.Values)
            {
                if (provider.TryGetFormatByName(name, out var format) && !RichFieldFormat.IsNullOrEmpty(format))
                    return format;
            }

            return null;
        }

        public RichFieldFormatType? GetFormatTypeForValue(string formatValue)
        {
            if (string.IsNullOrEmpty(formatValue))
                return null;

            // Check cache first
            if (_formatCache.TryGetValue(formatValue, out var cachedFormat))
                return cachedFormat.FormatType;

            // If not in cache, check providers
            foreach (var provider in _providers.Values)
            {
                var allFormats = provider.GetAllFormats();
                if (allFormats.Any(f => f.Value == formatValue))
                    return provider.FormatType;
            }

            return null;
        }

        public RichFieldFormat? GetFormatForValue(RichFieldFormatType formatType, string formatValue)
        {
            return TryGetFormatForValue(formatType, formatValue, out var format) ? format : null;
        }

        public bool TryGetFormatForValue(RichFieldFormatType formatType, string formatValue, out RichFieldFormat resultFormat)
        {
            resultFormat = RichFieldFormat.Empty;

            if (string.IsNullOrEmpty(formatValue))
                return false;

            // Check cache first
            if (_formatCache.TryGetValue(formatValue, out var cachedFormat))
            {
                resultFormat = cachedFormat;
                return !RichFieldFormat.IsNullOrEmpty(resultFormat);
            }

            // If not in cache, ask the provider
            var provider = GetProvider(formatType);
            if (provider is null)
                return false;

            return provider.TryGetFormatForValue(formatValue, out resultFormat);
        }

        public RichSwitchType GetSwitchType(string switchValue)
        {
            if (string.IsNullOrEmpty(switchValue))
                return RichSwitchType.None;

            // Check cache first
            if (_formatCache.TryGetValue(switchValue, out var cachedFormat))
            {
                return cachedFormat.FormatType switch
                {
                    RichFieldFormatType.Numeric => RichSwitchType.Numeric,
                    RichFieldFormatType.DateTime => RichSwitchType.DateTime,
                    RichFieldFormatType.General => RichSwitchType.General,
                    _ => RichSwitchType.FieldSpecific
                };
            }

            // If not in cache, check providers
            foreach (var provider in _providers.Values)
            {
                var allFormats = provider.GetAllFormats();
                if (allFormats.Any(f => f.Value == switchValue))
                {
                    return provider.FormatType switch
                    {
                        RichFieldFormatType.Numeric => RichSwitchType.Numeric,
                        RichFieldFormatType.DateTime => RichSwitchType.DateTime,
                        RichFieldFormatType.General => RichSwitchType.General,
                        _ => RichSwitchType.FieldSpecific
                    };
                }
            }

            return RichSwitchType.FieldSpecific;
        }

        public List<string> GetAvailableFormatValues(Type dataType)
        {
            var formats = GetAvailableFormats(dataType);
            return formats.Select(f => f.Value).Where(v => !string.IsNullOrEmpty(v)).ToList();
        }

        public List<RichFieldFormat> GetAvailableFormats(Type dataType)
        {
            if (dataType == null)
                return new List<RichFieldFormat>();

            var allFormats = new List<RichFieldFormat>();

            foreach (var provider in _providers.Values)
            {
                var formats = provider.GetAvailableFormats(dataType);
                allFormats.AddRange(formats);
            }

            return allFormats;
        }
        #endregion

    }
}
