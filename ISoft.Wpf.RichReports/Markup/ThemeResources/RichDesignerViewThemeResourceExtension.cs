namespace ISoft.Wpf.RichReports.Markup.ThemeResources
{
    public class RichDesignerViewThemeResourceExtension : ThemeDynamicResourceExtensionBase<RichDesignerViewThemeKeys>
    {
        protected override object GetResourceKey(RichDesignerViewThemeKeys Id)
        {
            return new RichDesignerViewThemeKeyExtension() { ResourceId = Id };
        }
    }
}
