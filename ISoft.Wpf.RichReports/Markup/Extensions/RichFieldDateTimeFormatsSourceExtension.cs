using ISoft.RichReportDesigner.Core.Fields.Format;
using System;

namespace ISoft.RichReportDesigner.UI.Markup.Extensions
{
    public sealed class RichFieldDateTimeFormatsSourceExtension : RichFieldFormatsSourceExtensionBase
    {
        protected override RichFieldFormatType FormatType => RichFieldFormatType.DateTime;
    }
}
