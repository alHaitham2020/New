using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReportDesigner.Core.Fields.Display;
using ISoft.Utils.Helpers;
using System;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public abstract class ChangeFieldCodeCommandBase<T> : FieldBasedCommand
    {
        protected ChangeFieldCodeCommandBase(IRichEditControl control) : base(control, null) { }

        protected ChangeFieldCodeCommandBase(IRichEditControl control, Field field) : base(control, field) { }

        #region Ovverieds
      
        protected sealed override void ExecuteCore(ICommandUIState state)
        {
            try
            {
                if (state is IValueBasedCommandUIState<T> valueState)
                {
                    ChangeValueCore(valueState);
                }
            }
            catch (Exception ex)
            {
                RunTimeLogger.CurrentLogger.LogException(ex);
            }
        }

        protected sealed override void UpdateUIStateCore(ICommandUIState state)
        {
            base.UpdateUIStateCore(state);

            if (state is IValueBasedCommandUIState<T> valueState)
            {
                valueState.Value = GetValue();
            }
        }

        protected override void OnApplyCommandRestriction(ICommandUIState state)
        {
            base.OnApplyCommandRestriction(state);

            var allowChange = CanChangeFieldCode(Field);

            ApplyCommandRestrictionOnEditableControl(state, DocumentModel.DocumentCapabilities.Fields, allowChange);

            if (state.Visible)
                state.Visible = allowChange;
        }

        public sealed override ICommandUIState CreateDefaultCommandUIState() => new DefaultValueBasedCommandUIState<T>();
        #endregion

        #region Protected Actions

        protected IRichDisplayField GetDataField(Field field)
        {
            var name = field?.CachedMergeFieldName;
            if (name == null) return null;
            
            return Control.Owner.DisplayFields.FindDataFieldByPath(name);
        }

        protected IRichDisplayField GetInnerDisplayField(Field field)
        {
            if (field == null) return null;

            var innerField = Helper.GetInnerCodeFieldOrField(field);

            if (innerField == null) return null;

            var fieldCode = ScanFieldCode(innerField);
            if (fieldCode == null) return null;

            return Control.Owner.DisplayFields.FindFieldByKey(fieldCode.FieldKey);
        }

        protected IRichDisplayField GetDisplayField(Field field)
        {
            if (field == null) return null;

            var fieldCode = ScanFieldCode(field);
            if (fieldCode == null) return null;
           
            return Control.Owner.DisplayFields.FindFieldByKey(fieldCode.FieldKey);
        }

        protected virtual bool CanChangeFieldCode(Field field) => field != null;

        #endregion

        #region Abstract
        protected abstract T GetValue();

        protected abstract void ChangeValueCore(IValueBasedCommandUIState<T> state);
        #endregion
    }
}
