namespace ISoft_RichReports.Editor.Data
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
