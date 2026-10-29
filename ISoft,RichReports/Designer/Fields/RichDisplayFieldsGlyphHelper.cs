using ISoft.RichReports.Data.Fields;
using ISoft.RichReports.Fields;
using System;

namespace ISoft.RichReports.Designer.Fields
{
    public static class RichDisplayFieldsGlyphHelper
    {
        public static Uri GetFieldImageUri(this RichFieldDataTypeCategory category)
        {
            return category switch
            {
                RichFieldDataTypeCategory.String => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Spreadsheet/Text2.svg"),
                RichFieldDataTypeCategory.Numeric => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Spreadsheet/AccountingNumberFormat.svg"),
                RichFieldDataTypeCategory.DateTime => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Spreadsheet/ShortDate.svg"),
                RichFieldDataTypeCategory.Boolean => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Spreadsheet/3SymbolsCircled.svg"),
                RichFieldDataTypeCategory.Image => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Dashboards/Images.svg"),
                _ => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Icon Builder/Actions_Question.svg"),
            };
        }

        public static Uri GetGroupImageUri(this RichFieldGroupCategory category)
        {
            return category switch
            {
                RichFieldGroupCategory.SystemFields => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Business Objects/BO_Address.svg"),
                RichFieldGroupCategory.DocumentProperties => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Spreadsheet/AccountingNumberFormat.svg"),
                RichFieldGroupCategory.CustomProperties => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Spreadsheet/ShortDate.svg"),
                RichFieldGroupCategory.DataFields => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Icon Builder/Actions_Database.svg"),
                _ => DevexpressImagesHelper.GetDxImageUri(@"SvgImages/Icon Builder/Actions_Question.svg"),
            };
        }
    }
}
