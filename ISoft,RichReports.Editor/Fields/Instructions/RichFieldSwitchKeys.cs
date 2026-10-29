namespace ISoft.RichReports.Editor.Fields.Instructions
{
    public static class RichFieldSwitchKeys
    {
        public class GeneralFormat
        {
            public const string MERGEFORMAT = "MERGEFORMAT";
        }

        public class DateField
        {
            public const string UseLunarCalendar = "h";

            public const string UseSakaEraCalendar ="s";

            public const string UseLastFormat = "l";
        }

        public class ImageBasedField
        {
            public const string ImageWidth = "w";

            public const string ImageHeight = "h";
        }

        public class MailMergeField : ImageBasedField
        {
            public const string Before = "b";

            public const string After = "f";

            public const string Barcode = "p";
        }

        public class IncludePictureField : ImageBasedField
        {
            public const string SuppressStoreGraphicsDataWithDocument = "d";

            public const string GraphicsFilter = "c";
        }
    }
}
