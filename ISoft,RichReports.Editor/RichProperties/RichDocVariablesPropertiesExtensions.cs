using ISoft.Office.Barcode;
using ISoft.Office.Images;
using ISoft.RichReports.Editor.Fields.Format;
using ISoft.Utils.Constants;
using System;

namespace ISoft.RichReports.Editor.RichProperties
{
    public static class RichDocVariablesPropertiesExtensions
    {
        public static void AddProperty(
            this IRichDocumentVariablesProperties properties,
            string name,
            string displayName,
            Type type,
            object value,
            object defaultValue,
            RichFieldFormat? format = null,
            bool visible = true)
        {
            properties.AddProperty(new RichDocumentVariableProperty(name, displayName, value, type, defaultValue, format, visible));
        }

        public static void AddProperty<T>(
            this IRichDocumentVariablesProperties properties,
            string name,
            string displayName,
            T value,
            T defaultValue,
            RichFieldFormat? format = null,
            bool visible = true)
        {
            properties.AddProperty(name, displayName, typeof(T), value, defaultValue, format, visible);
        }

        public static void AddBase64ImageProperty(
          this IRichDocumentVariablesProperties properties,
          string name,
          string displayName,
          byte[] image,
          bool visible = true)
        {
            var imageUri = ImageUri.CreateBase64(image);
            properties.AddImageProperty(name, displayName, imageUri, visible);
        }

        public static void AddResourceImageProperty(
          this IRichDocumentVariablesProperties properties,
          string name,
          string displayName,
          string resourcePath,
          bool visible = true)
        {
            var imageUri = ImageUri.CreateResource(resourcePath);
            properties.AddImageProperty(name, displayName, imageUri, visible);
        }

        public static void AddFileImageProperty(
          this IRichDocumentVariablesProperties properties,
          string name,
          string displayName,
          string filePath,
          bool visible = true)
        {
            var imageUri = ImageUri.CreateFile(filePath);
            properties.AddImageProperty(name, displayName, imageUri, visible);
        }

        public static void AddBarcodeImageProperty(
          this IRichDocumentVariablesProperties properties,
          string name,
          string displayName,
          BarcodeType barcodeType,
          string content,
          bool visible = true)
        {
            var imageUri = ImageUri.CreateBarcode(content, barcodeType);
            properties.AddImageProperty(name, displayName, imageUri, visible);
        }

        public static void AddImageProperty(
           this IRichDocumentVariablesProperties properties,
           string name,
           string displayName,
           ImageUri imageUri,
           bool visible = true)
        {
            if (ImageUri.IsNullOrEmpty(imageUri))
            {
                throw new ArgumentNullException(nameof(imageUri));
            }

            properties.AddProperty(name, displayName, imageUri, ImageUri.Empty, null, visible);
        }

        public static void AddStringProperty(this IRichDocumentVariablesProperties properties, string name, string displayName, string value)
        {
            properties.AddProperty(name, displayName, value, string.Empty);
        }

        public static void AddDateTimeProperty(this IRichDocumentVariablesProperties properties, string name, string displayName, DateTime value)
        {
            var format = RichFieldFormatHelper.GetDateTimeFormat(RichFieldDateTimeFormatType.DateTime);
            properties.AddProperty(name, displayName, value, DefaultNullDataTypes.NullDateTime, format);
        }

        public static void AddDateOnlyProperty(this IRichDocumentVariablesProperties properties, string name, string displayName, DateOnly value)
        {
            var format = RichFieldFormatHelper.GetDateTimeFormat(RichFieldDateTimeFormatType.ShortDate);
            properties.AddProperty(name, displayName, value, DefaultNullDataTypes.NullDateOnly, format);
        }

        public static void AddTimeOnlyProperty(this IRichDocumentVariablesProperties properties, string name, string displayName, TimeOnly value)
        {
            var format = RichFieldFormatHelper.GetDateTimeFormat(RichFieldDateTimeFormatType.ShortTime);
            properties.AddProperty(name, displayName, value, DefaultNullDataTypes.NullTimeOnly, format);
        }
    }

}
