using DevExpress.Mvvm;
using ISoft.Core.UI.Helpers;
using ISoft.Office.Barcode;
using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace ISoft.RichReportDesigner.UI.Markup.Extensions
{
    [MarkupExtensionReturnType(typeof(IEnumerable<EnumMemberInfo>))]
    public class RichBarcodeTypeSourceExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
           return EnumEditSettingsHelper.GetEnumItemsSource<BarcodeType>(false);
        }
    }
}
