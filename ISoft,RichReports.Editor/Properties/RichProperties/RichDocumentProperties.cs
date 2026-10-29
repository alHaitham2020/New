using ISoft.RichReports.Editor.Fields.Formatting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ISoft.RichReports.Editor.Properties.RichProperties
{
    /// <summary>
    /// contains all the properties of type RichDocumentProperty for each devexpress DocumentProperties (this class can be inherted for custom customize
    /// </summary>
    public class RichDocumentProperties : IRichDocumentProperties
    {
        #region Fields
        private readonly Dictionary<RichDocumentPropertyId, IRichDocumentProperty> _properties;
        private readonly IRichDocumentPropertyDisplayNameProvider _displayNameProvider;
        #endregion

        public RichDocumentProperties(IRichDocumentPropertyDisplayNameProvider displayNameProvider = null)
        {
            _displayNameProvider = displayNameProvider ?? new RichDocumentPropertyDisplayNameProvider(CultureInfo.CurrentUICulture.Name);
            _properties = new Dictionary<RichDocumentPropertyId, IRichDocumentProperty>();
            InitializeProperties();
        }

        #region Proptected Actions
        protected virtual void InitializeProperties()
        {
            AddProperty(RichDocumentPropertyId.Revision, "Revision", GetDisplayName(RichDocumentPropertyId.Revision), "0", typeof(int));

            AddStringProperty(RichDocumentPropertyId.Category, "Category", GetDisplayName(RichDocumentPropertyId.Category));
            AddStringProperty(RichDocumentPropertyId.ContentStatus, "ContentStatus", GetDisplayName(RichDocumentPropertyId.ContentStatus));
            AddStringProperty(RichDocumentPropertyId.ContentType, "ContentType", GetDisplayName(RichDocumentPropertyId.ContentType));
            AddStringProperty(RichDocumentPropertyId.Author, "Author", GetDisplayName(RichDocumentPropertyId.Author));
            AddStringProperty(RichDocumentPropertyId.Creator, "Creator", GetDisplayName(RichDocumentPropertyId.Creator));
            AddStringProperty(RichDocumentPropertyId.Description, "Description", GetDisplayName(RichDocumentPropertyId.Description));
            AddStringProperty(RichDocumentPropertyId.Identifier, "Identifier", GetDisplayName(RichDocumentPropertyId.Identifier));
            AddStringProperty(RichDocumentPropertyId.Keywords, "Keywords", GetDisplayName(RichDocumentPropertyId.Keywords));
            AddStringProperty(RichDocumentPropertyId.Language, "Language", GetDisplayName(RichDocumentPropertyId.Language));
            AddStringProperty(RichDocumentPropertyId.LastModifiedBy, "LastModifiedBy", GetDisplayName(RichDocumentPropertyId.LastModifiedBy));
            AddStringProperty(RichDocumentPropertyId.Subject, "Subject", GetDisplayName(RichDocumentPropertyId.Subject));
            AddStringProperty(RichDocumentPropertyId.Title, "Title", GetDisplayName(RichDocumentPropertyId.Title));
            AddStringProperty(RichDocumentPropertyId.Version, "Version", GetDisplayName(RichDocumentPropertyId.Version));
            AddStringProperty(RichDocumentPropertyId.Application, "Application", GetDisplayName(RichDocumentPropertyId.Application));
            AddStringProperty(RichDocumentPropertyId.AppVersion, "AppVersion", GetDisplayName(RichDocumentPropertyId.AppVersion));
            AddStringProperty(RichDocumentPropertyId.Company, "Company", GetDisplayName(RichDocumentPropertyId.Company));
            AddStringProperty(RichDocumentPropertyId.Manager, "Manager", GetDisplayName(RichDocumentPropertyId.Manager));
            AddStringProperty(RichDocumentPropertyId.Template, "Template", GetDisplayName(RichDocumentPropertyId.Template), null, true);

            AddDateTimeProperty(RichDocumentPropertyId.LastPrinted, "LastPrinted", GetDisplayName(RichDocumentPropertyId.LastPrinted));
            AddDateTimeProperty(RichDocumentPropertyId.Modified, "Modified", GetDisplayName(RichDocumentPropertyId.Modified));
            AddDateTimeProperty(RichDocumentPropertyId.Created, "Created", GetDisplayName(RichDocumentPropertyId.Created));
        }

        protected virtual string GetDisplayName(RichDocumentPropertyId propertyId)
        {
            return _displayNameProvider.GetDisplayName(propertyId);
        }

        protected void AddStringProperty(
           RichDocumentPropertyId id,
           string name,
           string displayName,
           RichFieldFormat? format = null,
           bool visible = false)
        {
            AddProperty(
                id: id,
                name: name,
                displayName: displayName,
                defaultValue: string.Empty,
                propertyType: typeof(string),
                format: format,
                visible: visible);
        }

        protected void AddDateTimeProperty(
           RichDocumentPropertyId id,
           string name,
           string displayName,
           RichFieldFormat? format = null,
           bool visible = false)
        {
            AddProperty(
                id: id,
                name: name,
                displayName: displayName,
                defaultValue: DefaultNullDataTypes.NullDateTime,
                propertyType: typeof(DateTime),
                format: format.GetFormatOrDefault(typeof(DateTime)),
                visible: visible);
        }

        protected void AddProperty(
            RichDocumentPropertyId id,
            string name,
            string displayName,
            object defaultValue,
            Type propertyType,
            RichFieldFormat? format = null,
            bool visible = false)
        {
            var property = new RichDocumentProperty(id, name, displayName, defaultValue, propertyType, format, visible);
            _properties.Add(id, property);
        }

        #endregion

        #region IRichDocumentProperties Implemnetation
        IRichDocumentProperty IRichDocumentProperties.GetProperty(RichDocumentPropertyId id)
        {
            return _properties.TryGetValue(id, out var property) ? property : null;
        }

        IEnumerable<IRichDocumentProperty> IRichDocumentProperties.GetAllProperties()
        {
            return _properties.Values;
        }

        IEnumerable<IRichDocumentProperty> IRichProperties<IRichDocumentProperty>.GetVisibleProperties()
        {
            return _properties.Values.Where(p => p.IsVisible);
        }

        void IRichDocumentProperties.UpdatePropertyVisiblity(RichDocumentPropertyId id, bool visible)
        {
            var property = ((IRichDocumentProperties)this).GetProperty(id);
            if (property == null) throw new InvalidOperationException($"{id}");

            if (property is RichDocumentProperty p)
            {
                p.IsVisible = visible;
                _properties[id] = p;
            }
        }

        T IRichDocumentProperties.GetPropertyDefaultValue<T>(RichDocumentPropertyId id)
        {
            var property = ((IRichDocumentProperties)this).GetProperty(id);
            if (property == null)
            {
                return default;
            }

            try
            {
                return property.DefaultValue.TryConvertValue<T>(property.PropertyType);
            }
            catch
            {
                return default;
            }
        }

        bool IRichProperties<IRichDocumentProperty>.Contains(string name)
        {
            return ((IRichDocumentProperties)this).GetProperty(name) != null;
        }

        IRichDocumentProperty IRichProperties<IRichDocumentProperty>.GetProperty(string name)
        {
            KeyValuePair<RichDocumentPropertyId, IRichDocumentProperty>? entry = _properties.FirstOrDefault(delegate (KeyValuePair<RichDocumentPropertyId, IRichDocumentProperty> pair)
            {
                if (pair.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return true;
                else
                    return false;
            });

            return entry.HasValue ? entry.Value.Value : null;
        }

        bool IRichProperties<IRichDocumentProperty>.TryGetProperty(string name, out IRichDocumentProperty item)
        {
            item = ((IRichDocumentProperties)this).GetProperty(name);
            return item != null;
        }

        bool IRichProperties<IRichDocumentProperty>.HasVisibleProprties() => ((IRichDocumentProperties)this).GetVisibleProperties().Any();

        void IRichDocumentProperties.ForEachPropertyId(Action<IRichDocumentProperties, RichDocumentPropertyId> action)
        {
            RichDocumentPropertyId[] ids = new RichDocumentPropertyId[_properties.Count];
            _properties.Keys.CopyTo(ids, 0);

            foreach (var id in ids)
            {
                action(this, id);
            }
        }

        void IRichDocumentProperties.ForEachProperty(Action<IRichDocumentProperties, IRichDocumentProperty> action)
        {
            IRichDocumentProperty[] properties = new IRichDocumentProperty[_properties.Count];
            _properties.Values.CopyTo(properties, 0);

            foreach (var property in properties)
            {
                action(this, property);
            }
        }

        IRichDocumentProperty IRichProperties<IRichDocumentProperty>.this[string name] => ((IRichDocumentProperties)this).GetProperty(name);

        int IRichProperties<IRichDocumentProperty>.Count => _properties.Count;

        bool IRichProperties<IRichDocumentProperty>.HasProprties => _properties.Count > 0;

        IEnumerable<string> IRichProperties<IRichDocumentProperty>.Names => _properties.Values.Select(p => p.Name);

        #endregion
    }
}
