using DevExpress.Office.Utils;
using DevExpress.Office.Utils.Internal;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using ISoft.Office.Barcode;
using ISoft.Office.Images;

namespace ISoft.RichReports.Editor.Internal.Fields
{
    internal static class RichEditFieldBarcodeManager
    {
        public static CalculatedFieldValue InsertBarcodeImage(
            PieceTable sourcePieceTable,
            string value,
            BarcodeType barcodeType,
            int? imageWidth,
            int? imageHight)
        {
            var uri = ImageUri.CreateBarcode(value, barcodeType);

            UriBasedOfficeImage uriBasedOfficeImage = new(uri, imageWidth, imageHight, sourcePieceTable.DocumentModel, asyncImageLoading: true);
            InternalUriBasedOfficeImageHelper.SetSuppressStorePlaceholder(uriBasedOfficeImage, value: true);
            DocumentModel fieldResultModel = sourcePieceTable.DocumentModel.GetFieldResultModel();
            InsertInlinePictureInTargetModel(uriBasedOfficeImage, fieldResultModel, false);
            return new CalculatedFieldValue(fieldResultModel);
        }

        private static void InsertInlinePictureInTargetModel(
            OfficeImage image,
            DocumentModel targetModel,
            bool suppressStore)
        {
            InternalOfficeImageHelper.SetSuppressStore(image, suppressStore);
            InternalOfficeImageHelper.EnsureLoadComplete(image);

            ModelPicture modelPicture = new(targetModel.MainPieceTable);
            //modelPicture.ShapeProperties.OutlineType = OutlineType.Solid;
            //modelPicture.ShapeProperties.ShapeType = ShapePreset.RoundRect;
            //modelPicture.ShapeProperties.Outline.CompoundType = OutlineCompoundType.Double;
            //modelPicture.ShapeProperties.OutlineColor = DrawingColor.Create(targetModel.MainPieceTable, System.Drawing.Color.Red);
            modelPicture.BlipFill.Blip.Image = image;
            modelPicture.BlipFill.Stretch = true;
            targetModel.MainPieceTable.InsertDrawingPicture(DocumentLogPosition.Zero, modelPicture, inline: true);
        }
    }
}
