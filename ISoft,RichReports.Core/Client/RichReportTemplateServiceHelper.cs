using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Client
{
    public static class RichReportTemplateServiceHelper
    {
        //internal static string GenerateFieldName(this IRichReportTemplateService service)
        //{
        //    int index = 1;

        //    while (service..Any((XpfReportTitleItem w) => w.FieldName == $"{"item"}{index}"))
        //        while (collection.ContainsKey($"{"item"}{index}"))
        //    {
        //        index++;
        //    }

        //    return $"{"item"}{index}";
        //}


        //protected virtual string GetNewDocumentName()
        //{
        //    int result;
        //    int num = (from x in GetExistingDocuments()
        //               select x.Title into x
        //               where x.StartsWith("Report")
        //               select x.Substring("Report".Length) into x
        //               select int.TryParse(x, NumberStyles.None, CultureInfo.InvariantCulture, out result) ? result : 0 into x
        //               where x > 0
        //               select x).DefaultIfEmpty(0).Max();
        //    return "Report" + (num + 1).ToString(CultureInfo.InvariantCulture);
        //}

        public static string GetNewName(this IRichReportTemplateService service, int? reportId, string reportTitle)
        {
            var count = service.CountByType(reportId);

            return GetNewNameCore(count, reportTitle);
        }

        public static async Task<string> GetNewNameAsync(this IRichReportTemplateService service, int? reportId, string reportTitle)
        {
            var count = await service.CountByTypeAsync(reportId);

            return GetNewNameCore(count, reportTitle);
        }

        private static string GetNewNameCore(int count, string baseName)
        {
            return count == 0 ? baseName : $"{baseName} {count + 1}";
        }
    }
}
