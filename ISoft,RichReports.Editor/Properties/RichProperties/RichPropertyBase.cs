using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.DataModel;
using System;

namespace ISoft.RichReports.Editor.RichProperties
{
    public abstract class RichPropertyBase : ObjectBase, IRichProperty
    {
        #region Fields
        private string _name;
        private string _displayName;
        private object _defaultValue;
        private bool _isVisible;
        private RichFieldFormat? _format;
        private Type _propertyType;
        #endregion

        #region Proprties
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
            }
        }

        public object DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                _defaultValue = value;
            }
        }

        public RichFieldFormat? Format
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
            }
        }

        public Type PropertyType
        {
            get
            {
                return _propertyType;
            }
            set
            {
                _propertyType = value;
            }
        }

        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }

        public bool HasFormat => !RichFieldFormat.IsNullOrEmpty(Format);

        public bool AllowUseDefaultEmptyValue => true;

        #endregion

        public RichPropertyBase()
        {

        }

        public RichPropertyBase(
            string name,
            string displayName,
            object defaultValue,
            Type propertyType,
            RichFieldFormat? format = null,
            bool? visible = null)
        {
            _name = name;
            _displayName = displayName;
            _defaultValue = defaultValue;
            _propertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
            Format = format;
            IsVisible = visible ?? true;
        }  
    }
}
