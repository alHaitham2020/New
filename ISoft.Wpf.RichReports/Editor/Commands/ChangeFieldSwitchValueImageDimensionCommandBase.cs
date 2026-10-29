namespace ISoft.Wpf.RichReports.Editor.Commands
{
    public abstract class ChangeFieldSwitchValueImageDimensionCommandBase<T> : ChangeFieldSwitchValueCommand<T>
    {
        protected ChangeFieldSwitchValueImageDimensionCommandBase(IRichEditControl control) : base(control) { }

        protected ChangeFieldSwitchValueImageDimensionCommandBase(IRichEditControl control, Field field) : base(control, field) { }

        protected override bool CanChangeFieldCode(Field field)
        {
            if (field == null) return false;

            // is not inner field (field.Parent == null or use (field.GetFieldLocation() == FieldLocation.OrdinaryText))
            return field.Parent == null && 
                  (field.CachedFieldType == MergefieldField.FieldType || 
                   field.CachedFieldType == RichExpressionEditorCalculatedField.FieldType || 
                   field.CachedFieldType == IncludepictureField.FieldType);
        }

        protected override void ChangeValueCore(IValueBasedCommandUIState<T> state)
        {
            if (Field.CachedFieldType == IncludepictureField.FieldType && Helper.HasInnerCodeFields(Field))
            {
                UpdateNestedSwtichValue(Field, SwitchKey, state.Value.ConvertValueToString());
            }
            else
            {
                if (Field.CachedFieldType == IncludepictureField.FieldType || 
                    Field.CachedFieldType == MergefieldField.FieldType || 
                    Field.CachedFieldType == RichExpressionEditorCalculatedField.FieldType)
                {
                    UpdateSwitchValue(Field, state);
                }
            }
        }
    }
}
