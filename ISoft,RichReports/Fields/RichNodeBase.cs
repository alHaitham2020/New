using System;
using System.Collections.Generic;
using System.Linq;

namespace ISoft.RichReports.Fields
{
    public abstract class RichNodeBase : ObservableObject, IRichNode
    {
        #region Fields
        private string _name;
        private string _displayName;
        private string _path;
        private bool _isVisible = true;
        private Type _dataType;
        #endregion

        #region Proprties
        public string Name
        {
            get => _name;
            set { _name = value; }
        }

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        public string Path
        {
            get => _path;
            set { _path = value; }
        }

        public Type DataType
        {
            get => _dataType;
            set { _dataType = value; }
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public abstract bool IsGroup { get; }

        public virtual IRichFieldGroup ParentGroup { get; set; }
        
        #endregion

        public Type GetActualDataType()
        {
            var dataType = DataType;
            if(dataType == null) return null;

            return dataType.GetUnderlyingTypeOrType();
        }

        public IEnumerable<IRichNode> GetAncestors()
        {
            var current = ParentGroup;
            while (current != null)
            {
                yield return current;
                current = current.ParentGroup;
            }
        }

        public bool IsDescendantOf(IRichNode node)
        {
            return GetAncestors().Any(a => a.Equals(node));
        }
    }
}
