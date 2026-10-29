using ISoft.RichReports.Editor.Fields.Instructions;
using ISoft.RichReports.Editor.Internal.Fields;
using ISoft.RichReports.Editor.Services.Abstractions;
using System.Collections.Generic;
using System.IO;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    public class RichEditIncludePictureField : IncludepictureField, IRichEditField
    {
        private static readonly Dictionary<string, bool> switchesWithArgument
                   = CreateSwitchesWithArgument(
                       RichFieldSwitchKeys.IncludePictureField.GraphicsFilter,
                       RichFieldSwitchKeys.ImageBasedField.ImageWidth,
                       RichFieldSwitchKeys.ImageBasedField.ImageHeight);

        protected override Dictionary<string, bool> SwitchesWithArguments => switchesWithArgument;

        private string _displayName;

        private int? _width;

        private int? _hight;

        public int? Width => _width;

        public int? Hight => _hight;

        public new static CalculatedFieldBase Create()
        {
            return new RichEditIncludePictureField();
        }

        public override void Initialize(PieceTable pieceTable, InstructionCollection instructions)
        {
            base.Initialize(pieceTable, instructions);
            _width = instructions.GetNullableInt(RichFieldSwitchKeys.ImageBasedField.ImageWidth) ?? 100;
            _hight = instructions.GetNullableInt(RichFieldSwitchKeys.ImageBasedField.ImageHeight) ?? 100;

            IRichDataFieldsManagerService service = pieceTable.DocumentModel.GetService<IRichDataFieldsManagerService>();
            _displayName = service?.GetFieldDisplayName(DocumentName); // inner field display name
        }

        protected override FieldMailMergeType MailMergeType()
        {
            return FieldMailMergeType.MailMerge;
        }

        protected override string GetDefaultMailMergeFieldText()
        {
            if (!string.IsNullOrEmpty(DocumentName) && DocumentName.Trim().StartsWith("["))
                return DocumentName;

            string fieldDisplayName = this.CalculateDisplayName(_displayName, DocumentName);

            if (string.IsNullOrWhiteSpace(fieldDisplayName))
            {
                return GetDefaultInvalidValueText();
            }

            return fieldDisplayName;
        }

        public override CalculatedFieldValue GetCalculatedValueCore(PieceTable sourcePieceTable, MailMergeDataMode mailMergeDataMode, Field documentField)
        {
            List<string> list = [DocumentName];
            string currentFileName = sourcePieceTable.DocumentModel.DocumentSaveOptions.CurrentFileName;
            try
            {
                if (!string.IsNullOrEmpty(currentFileName))
                {
                    list.Add(Path.Combine(Path.GetDirectoryName(currentFileName), DocumentName));
                }
            }
            catch
            {

            }

            UriBasedOfficeImage uriBasedOfficeImage = new([.. list], _width, _hight, sourcePieceTable.DocumentModel, asyncImageLoading: true);
            InternalUriBasedOfficeImageHelper.SetSuppressStorePlaceholder(uriBasedOfficeImage, value: true);
            DocumentModel fieldResultModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
            InsertInlinePictureInTargetModel(uriBasedOfficeImage, fieldResultModel, SuppressStoreGraphicsDataWithDocument && !sourcePieceTable.DocumentModel.ModelForMailMergeRecord);
            return new CalculatedFieldValue(fieldResultModel);
        }
    }
}
