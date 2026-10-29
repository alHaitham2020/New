using DevExpress.XtraRichEdit.Model;

namespace ISoft.RichReports.Editor.Data
{
    public class RichEditDataController : RichDataController
    {
        public static RichEditDataController CreateDefault()
        {
            return new RichEditDataController()
            {
                AllowIEnumerableDetails = true,
                AllowDetailDescriptors = true,
            };
        }
    }
}
