using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReportDesigner.Core.Fields.Format;
using ISoft.RichReportDesigner.Core.Fields.Switches;
using ISoft.RichReportDesigner.Core.RichEdit;
using ISoft.Utils.Helpers;
using System;

namespace ISoft.RichReportDesigner.UI.RichEdit.Commands
{
    public abstract class ChangeFieldSwitchValueCommand<T> : ChangeFieldCodeCommandBase<T>
    {
        protected ChangeFieldSwitchValueCommand(IRichEditControl control) : base(control) { }

        protected ChangeFieldSwitchValueCommand(IRichEditControl control, Field field) : base(control, field) { }

        protected abstract string SwitchKey { get; }

        protected abstract T DefaultValue { get; }

        #region Ovverides
        /// <summary>
        /// Retrieves the current value of the switch from the field code.
        /// Returns DefaultValue if field is null or switch is not found.
        /// </summary>
        protected sealed override T GetValue()
        {
            if (Field == null)
            {
                return DefaultValue;
            }

            var fieldSwitch = FindFieldSwitch(Field);
            if (fieldSwitch == null)
            {
                return DefaultValue;
            }

            return fieldSwitch.GetValue(DefaultValue);
        }

        #endregion

        #region Protected Actions
        /// <summary>
        /// Updates the switch value by rebuilding the entire field code.
        /// Use for complex field types where piece-table manipulation is unreliable.
        /// </summary>
        protected void UpdateSwitchValue(Field field, IValueBasedCommandUIState<T> state)
        {
            if (field == null) return;
            try
            {
                var fieldCode = ScanFieldCode(field);// only scan with clean code
                if (fieldCode == null) return;

                var switchValue = state.Value.ConvertValueToString();
                if (IsEmptyStringValue(switchValue))
                {
                    fieldCode.Switches.RemoveByKey(SwitchKey);
                }
                else
                {
                    fieldCode.Switches.AddOrUpdateSwitch(SwitchKey, switchValue);
                }

                var newCode = fieldCode.Build();
                if (!string.IsNullOrEmpty(newCode))
                {
                    //var interval = field.GetCodeInterval(ActivePieceTable);
                    //var range = Control.Document.CreateRange(interval.Start.ToInt(), interval.Length);
                    //Control.Document.ReplaceFieldCode(range, newCode);
                    Helper.ReplaceFieldCode(field, newCode);
                }
            }
            catch (Exception e)
            {
                RunTimeLogger.CurrentLogger.LogException(e);
            }
        }

        /// <summary>
        /// Performs surgical update of a switch value in the field code using piece-table operations.
        /// Supports three operations: delete (when value is empty), insert (when switch doesn't exist),
        /// and update (when switch exists with a different value).
        /// </summary>
        protected void UpdateNestedSwtichValue(Field field, string switchKey, string switchValue, bool addIfNotFound = true)
        {
            if (field == null) return;

            var rawCodeText = Helper.GetOrginalFieldCode(field);
            if (string.IsNullOrEmpty(rawCodeText)) return;

            var switchInfo = Scanner.GetSwitchInfo(rawCodeText, switchKey);
            var startPosition = field.GetCodeStartPosition(ActivePieceTable);
            var isEmpty = IsEmptyStringValue(switchValue);

            // Delete: Switch exists but new value is empty/invalid
            if (switchInfo.IsValid && isEmpty)
            {
                var deleteStart = startPosition.LogPosition + switchInfo.CodeInfo.StartIndex;
                var deleteLength = switchInfo.CodeInfo.Length + switchInfo.ValueInfo.Length + 2; // +2 for spaces
                Helper.DeleteTextFromFieldCode(field, deleteStart, deleteLength);
                //var range = Control.Document.CreateRange(deleteStart.ToInt(), deleteLength);
                //Control.Document.Delete(range);
                return;
            }

            // Insert: Switch doesn't exist and new value is valid
            if (switchInfo.IsEmpty && !isEmpty && addIfNotFound)
            {
                var switchCode = RichFieldSwitch.CreateSwitch(switchKey, switchValue).Build();
                var endPosition = field.GetCodeEndPosition(ActivePieceTable);
                Helper.InsertTextIntoField(field, endPosition.LogPosition, switchCode);
                //var insertPosition = Control.Document.CreatePosition(endPosition.LogPosition.ToInt());
                //Control.Document.InsertText(insertPosition, switchCode);
                return;
            }

            // Update: Switch exists and new value is valid
            if (!switchInfo.ValueInfo.IsEmpty && !isEmpty)
            {
                var updateStart = startPosition.LogPosition + switchInfo.ValueInfo.StartIndex;
                var sv = RichFieldSwitchCodeHelper.NormalizeSwitchValue(switchValue);
                Helper.UpdateTextInFieldCode(field, updateStart, switchInfo.ValueInfo.Length, sv);
                //var rng = Control.Document.CreateRange(updateStart.ToInt(), switchInfo.ValueInfo.Length);
                //Control.Document.Replace(rng, sv);
            }
        }

        /// <summary>
        /// Determines if a switch value should be considered empty/invalid.
        /// Override to customize the empty value logic for specific switch types.
        /// </summary>
        protected virtual bool IsEmptyStringValue(string value)
        {
            return  string.Equals(value, DefaultValue.ConvertValueToString(), StringComparison.OrdinalIgnoreCase);
        }

        protected IRichFieldSwitch FindFieldSwitch(Field field)
        {
            if(field == null) return null;

            return Scanner.FindSpecificSwitch(Helper.GetCleanCode(field), SwitchKey);
        }

        protected RichFieldFormatType GetFieldFormatType(Field field)
        {
            if (field == null)
            {
                return RichFieldFormatType.None;
            }

            var fieldSwitch = FindFieldSwitch(field);
            if (fieldSwitch == null)
            {
                return RichFieldFormatType.None;
            }

            return fieldSwitch.IsFormatSwitch ? fieldSwitch.SwitchType.ToFormatType() : RichFieldFormatType.None;
        }

        //    //private void ReplaceNative(ICommandUIState state)
        //    //{
        //    //    var vl = state as IValueBasedCommandUIState<FieldBarcodeType>;

        //    //    DocumentRange selection = Control.Document.Selection;
        //    //    var serv = new RichFieldManagerService();
        //    //    if (serv != null)
        //    //    {
        //    //        var field = Control.Document.Fields.FirstOrDefault(f => f.Range.Contains(selection.Start));

        //    //        if (field != null)
        //    //        {
        //    //            serv.UpdateSwitchValue(Control.Document, field, RichMergeFieldFormater.BarcodeKey, $"{vl.Value}", true);
        //    //        }
        //    //    }
        //    //}
        #endregion
    }
}
