using ISoft.RichReports.Core.Abstractions.Templates;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Templates.Serialization
{
    public static class RichReportTemplateSerializationHelper
    {
        public static string Serialize(RichReportTemplate template)
            => JsonSerializationHelper.SerializeToJson(template);

        public static IRichReportTemplate Deserialize(string layout)
            => JsonSerializationHelper.DeserializeFromJson<RichReportTemplate>(layout);

        public static void SerializeToFile(IRichReportTemplate template, string path)
            => JsonSerializationHelper.SerializeToFile(template, path);

        public static Task<bool> SerializeToFileAsync(IRichReportTemplate template, string path)
           => JsonSerializationHelper.SerializeToFileAsync(template, path);

        public static RichReportTemplate DeserializeFromFile(string path)
            => JsonSerializationHelper.DeserializeFromFile<RichReportTemplate>(path);

        public static Task<RichReportTemplate> DeserializeFromFileAsync(string path)
            => JsonSerializationHelper.DeserializeFromFileAsync<RichReportTemplate>(path);
    }
}
