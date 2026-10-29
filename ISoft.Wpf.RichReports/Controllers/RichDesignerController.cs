using ISoft.RichReportDesigner.UI.Controls;
using ISoft.Utils.Helpers;

namespace ISoft.RichReportDesigner.UI.Controllers
{
    internal sealed class RichDesignerController
    {
        public RichDesignerFieldController Fields { get; }

        public RichDesignerTemplateController Template { get; }

        public RichDesignerCommandsHandler CommandsHandler { get; }

        public RichDesignerEditorController Editor { get; }

        public RichDesignerController(RichReportDesignerControl control)
        {
            ArgumentGuardHelper.ArgumentNotNull(control,nameof(control));

            Fields = new RichDesignerFieldController(control);
            Template = new RichDesignerTemplateController(control);
            CommandsHandler = new RichDesignerCommandsHandler(control, Template);
            Editor = new RichDesignerEditorController(control);
        }
    }
}
