using ISoft.RichReports.Core.Abstractions.Printing;

namespace ISoft.RichReports.Core.Printing
{
    public class DefaultRichReportPrintPermissionInfo : IRichReportPrintPermissionInfo
    {
        public bool AllowCreateNewTemplate => true;

        public bool AllowModefiyTemplate => true;

        public bool AllowDeleteTemplate => true;

        public bool AllowImportTemplete => true;

        public bool AllowExportTemplete => true;

    }
}
