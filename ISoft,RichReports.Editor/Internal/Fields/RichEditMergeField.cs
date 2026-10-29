using ISoft.RichReports.Editor.Data;
using ISoft.RichReports.Editor.Fields.Instructions;
using ISoft.RichReports.Editor.Internal.Fields;
using ISoft.RichReports.Editor.Services.Abstractions;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditMergeField : MergefieldField, IRichEditField
    {
        private const string NullValueBarcodeText = "[Null]";

        private const string InvalidValueText = "[Undefined Field]";

        private static readonly Dictionary<string, bool> switchesWithArgument 
            = CreateSwitchesWithArgument(
                RichFieldSwitchKeys.MailMergeField.Before,
                RichFieldSwitchKeys.MailMergeField.After,
                RichFieldSwitchKeys.MailMergeField.Barcode,
                RichFieldSwitchKeys.ImageBasedField.ImageWidth,
                RichFieldSwitchKeys.ImageBasedField.ImageHeight);

        protected override Dictionary<string, bool> SwitchesWithArguments => switchesWithArgument;

        private IRichFieldDataInfo _fieldInfo;

        private BarcodeType _barcodeType;

        private int? _barcodeWidth;

        private int? _barcodeHight;

        private bool _isBarcodeEnabled;

        public new static CalculatedFieldBase Create()
        {
            return new RichEditMergeField();
        }

        public override void Initialize(PieceTable pieceTable, InstructionCollection instructions)
        {
            base.Initialize(pieceTable, instructions);

            _barcodeWidth = instructions.GetNullableInt(RichFieldSwitchKeys.ImageBasedField.ImageWidth) ?? 100;
            _barcodeHight = instructions.GetNullableInt(RichFieldSwitchKeys.ImageBasedField.ImageHeight) ?? 100;
            _barcodeType = instructions.GetString(RichFieldSwitchKeys.MailMergeField.Barcode).TryConvert(BarcodeType.None);
            _isBarcodeEnabled = _barcodeType != BarcodeType.None;

            IRichDataFieldsManagerService service = pieceTable.DocumentModel.GetService<IRichDataFieldsManagerService>();
            _fieldInfo = service?.GetFieldInfo(DataFieldName);
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            string fieldDisplayName = this.CalculateDisplayName(_fieldInfo?.DisplayName, DataFieldName);

            if (string.IsNullOrWhiteSpace(fieldDisplayName))
            {
                return GetDefaultInvalidValueText();
            }

            return fieldDisplayName;
        }

        protected override string GetDefaultInvalidValueText() => InvalidValueText;

        protected override CalculatedFieldValue GetNullValue()
        {
            var value = _isBarcodeEnabled ? NullValueBarcodeText : string.Empty; // when barcode null then ist improtant to return empty string
            return new CalculatedFieldValue(value);
        }

        public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField)
        {
            var calculatedValue = base.GetCalculatedValueCore(sourcePieceTable, mailMergeDataMode, documentField);

            if (_isBarcodeEnabled)
            {
                // apply formating before barcode generation, because barcode generation is based on the text value
                ApplyFieldFormatting(calculatedValue, sourcePieceTable.DocumentModel.MailMergeOptions.CustomSeparators);

                return RichEditFieldBarcodeManager.InsertBarcodeImage(
                    sourcePieceTable,
                    calculatedValue.Text,
                    _barcodeType,
                    _barcodeWidth,
                    _barcodeHight);
            }

            return calculatedValue;
        }
    }
}
