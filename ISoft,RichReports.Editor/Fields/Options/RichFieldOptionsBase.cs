using ISoft.RichReports.Editor.Fields.Codes;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.Utils.DataModel;

namespace ISoft.RichReports.Editor.Fields.Options
{
    public interface IRichFieldOptions
    {
        RichEditFieldType FieldType { get; }

        bool CanApplyArgumentValue(string propertyName);

        bool CanApplySwitchValue(string propertyName);
    }

    public abstract class RichFieldOptionsBase : ObjectBase, IRichFieldOptions
    {
        public abstract RichEditFieldType FieldType { get; }

        protected virtual bool CanApplySwitchValue(string propertyName)
        {
            return true;
        }

        protected virtual bool CanApplyArgumentValue(string propertyName)
        {
            return true;
        }

        bool IRichFieldOptions.CanApplyArgumentValue(string propertyName)
        {
            return CanApplyArgumentValue(propertyName);
        }

        bool IRichFieldOptions.CanApplySwitchValue(string propertyName)
        {
            return CanApplySwitchValue(propertyName);
        }

        public virtual void Write(IRichFieldCode fieldCode)
        {

        }

        public virtual void Read(IRichFieldCode fieldCode)
        {

        }

        public virtual void Reset()
        {

        }
    }
}
