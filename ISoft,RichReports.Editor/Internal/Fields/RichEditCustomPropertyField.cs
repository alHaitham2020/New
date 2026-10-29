using DevExpress.Office.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.RichReports.Editor.Services;
using System;
using System.Collections.Generic;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditCustomPropertyField : RichEditCalculatedFieldBase
    {
        public static readonly string FieldType = "CUSTOMPROPERTY";

        protected override string FieldTypeName => FieldType;

        private static readonly Dictionary<string, bool> switchesWithArgument = CreateSwitchesWithArgument();

        protected static Dictionary<string, bool> SwitchesWithArgument => switchesWithArgument;

        protected override Dictionary<string, bool> SwitchesWithArguments => switchesWithArgument;

        private string _propertyName;

        private string _displayName;

        public static CalculatedFieldBase Create()
        {
            return new RichEditCustomPropertyField();
        }

        protected sealed override FieldMailMergeType MailMergeType()
        {
            return FieldMailMergeType.MailMerge;
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            string fieldDisplayName = this.CalculateDisplayName(_displayName, _propertyName);

            if (string.IsNullOrWhiteSpace(fieldDisplayName))
            {
                return GetDefaultInvalidValueText();
            }

            return fieldDisplayName;
        }

        public sealed override void Initialize(PieceTable pieceTable, InstructionCollection switches)
        {
            base.Initialize(pieceTable, switches);
            _propertyName = switches.GetArgumentAsString(0);

            IRichEditDocumentCustomPropertiesManagerService service = pieceTable.DocumentModel.GetService<IRichEditDocumentCustomPropertiesManagerService>();
            _displayName = service?.GetDisplayName(_propertyName);
        }

        public sealed override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField)
        {
            object value = GetPropertyValue(sourcePieceTable);

            if (value == null || value == DBNull.Value)
            {
                return GetNullValue();
            }

            return new CalculatedFieldValue(value);
        }

        protected virtual object GetPropertyValue(PieceTable sourcePieceTable)
        {
            if (string.IsNullOrEmpty(_propertyName))
            {
                return null;
            }

            CustomPropertyValue? propertyValue = sourcePieceTable.DocumentModel.CustomProperties[_propertyName]?.Value;
            if (propertyValue != null) 
                return propertyValue.Value.GetValue();

            return null;
        }

        protected virtual CalculatedFieldValue GetNullValue()
        {
            return new CalculatedFieldValue(string.Empty);
        }
    }
}
