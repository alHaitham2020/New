using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Instructions;

namespace ISoft.RichReports.Editor.Fields.Options
{
    /// <summary>
    /// Options for IncludePicture fields
    /// </summary>
    public class RichIncludePictureFieldOptions : RichImageFieldOptionsBase
    {
        public override RichEditFieldType FieldType => RichEditFieldType.IncludePictureField;

        [RichFieldArgument(0)]
        public string Uri { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.IncludePictureField.GraphicsFilter)]
        public string GraphicsFilter { get; set; }

        [RichFieldSwitch(RichFieldSwitchKeys.IncludePictureField.SuppressStoreGraphicsDataWithDocument)]
        public bool SuppressStore { get; set; }

        public RichIncludePictureFieldOptions WithUri(string uri)
        {
            var clone = this.CreateClone();
            clone.Uri = uri;
            return clone;
        }

        public static RichIncludePictureFieldOptions Create(string uri)
        {
            return new RichIncludePictureFieldOptions
            {
                Uri = uri,
                ImageHeight = 100,
                ImageWidth = 100,
                SuppressStore = false,
            };
        }

        public static RichIncludePictureFieldOptions CreateDefault()
        {
            return new RichIncludePictureFieldOptions
            {
                ImageHeight = 100,
                ImageWidth = 100,
                SuppressStore = false,
            };
        }
    }
}
