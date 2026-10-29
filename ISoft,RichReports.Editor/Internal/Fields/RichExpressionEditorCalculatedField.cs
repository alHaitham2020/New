using ISoft.RichReports.Editor.Fields.Instructions;
using ISoft.RichReports.Editor.Services.Abstractions;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichExpressionEditorCalculatedField : RichEditCalculatedFieldBase
    {
        public static readonly string FieldType = "EXPEDITORFIELD";

        private const string NullValueBarcodeText = "[Null]";

        private const string InvalidValueText = "[Undefined Field]";

        protected override string FieldTypeName => FieldType;

        private static readonly Dictionary<string, bool> switchesWithArgument
                   = CreateSwitchesWithArgument(
                       RichFieldSwitchKeys.MailMergeField.Barcode,
                       RichFieldSwitchKeys.ImageBasedField.ImageWidth,
                       RichFieldSwitchKeys.ImageBasedField.ImageHeight);

        protected override Dictionary<string, bool> SwitchesWithArguments => switchesWithArgument;

        public string FieldName => _fieldName;

        private string _fieldName;
        private string _displayName;
        private BarcodeType _barcodeType;
        private int? _barcodeWidth;
        private int? _barcodeHight;
        private bool _isBarcodeEnabled;

        public static CalculatedFieldBase Create()
        {
            return new RichExpressionEditorCalculatedField();
        }

        public override void Initialize(PieceTable pieceTable, InstructionCollection instructions)
        {
            base.Initialize(pieceTable, instructions);
            _fieldName = instructions.GetArgumentAsString(0) ?? null;

            _barcodeWidth = instructions.GetNullableInt(RichFieldSwitchKeys.ImageBasedField.ImageWidth) ?? 100;
            _barcodeHight = instructions.GetNullableInt(RichFieldSwitchKeys.ImageBasedField.ImageHeight) ?? 100;
            _barcodeType = instructions.GetString(RichFieldSwitchKeys.MailMergeField.Barcode).TryConvert(BarcodeType.None);
            _isBarcodeEnabled = _barcodeType != BarcodeType.None;

            IRichEditCalculatedFieldManagerService service = pieceTable.DocumentModel.GetService<IRichEditCalculatedFieldManagerService>();
            _displayName = service?.GetFieldDisplayName(_fieldName);
        }

        protected sealed override FieldMailMergeType MailMergeType()
        {
            return FieldMailMergeType.MailMerge;
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            string fieldDisplayName = this.CalculateDisplayName(_displayName, _fieldName);

            if (string.IsNullOrWhiteSpace(fieldDisplayName))
            {
                return GetDefaultInvalidValueText();
            }

            return fieldDisplayName;
        }

        protected override string GetDefaultInvalidValueText() => InvalidValueText;

        public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField)
        {
            var calculatedValue = GetValue(sourcePieceTable, mailMergeDataMode, documentField);

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

        private CalculatedFieldValue GetValue(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField)
        {
            IFieldDataService service = sourcePieceTable.DocumentModel.GetService<IFieldDataService>();
            object fieldValue = service.GetFieldValue(sourcePieceTable.DocumentModel.MailMergeProperties, _fieldName, false, mailMergeDataMode, sourcePieceTable, documentField);
            if (fieldValue == null || fieldValue == DBNull.Value)
            {
                return GetNullValue();
            }

            return new CalculatedFieldValue(fieldValue);
        }

        protected virtual CalculatedFieldValue GetNullValue()
        {
            var value = _isBarcodeEnabled ? NullValueBarcodeText : string.Empty; // when barcode null then ist improtant to return empty string
            return new CalculatedFieldValue(value);
        }
    }
}
