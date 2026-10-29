using ISoft.RichReports.Core.Templates;
using ISoft.RichReports.UI;
using System;
using System.Threading.Tasks;

namespace ISoft.Wpf.RichReports.DataModel
{
    public interface IXpfRichReport : IRichReport
    {
        IXpfRichReportTemplateItemsSource DefaultTemplatesSource { get; }

        IXpfRichReportTemplateItemsSource StoreTemplatesSource { get; }

        IRichReportTemplate DesignerTemplate { get; }

        Task NewTemplateAsync();

        Task CloneAsync(Guid? templateKey);

        Task EditTemplateAsync(Guid? templateKey);

        void EditTemplate(IRichReportTemplate template);

        void ShowTemplatesEditorDialog(string title = null);
    }
}
