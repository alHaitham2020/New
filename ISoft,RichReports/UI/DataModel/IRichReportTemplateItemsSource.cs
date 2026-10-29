using ISoft.Utils.Mvvm;
using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.UI.DataModel
{
    public interface IRichReportTemplateItemsSource
    {
        Task OnItemAddedAsync(Guid? key);

        Task OnItemUpdatedAsync(Guid? key);

        Task OnItemDeletedAsync(Guid? key);

        Task OnItemChangedAsync(DataChangeMessageType changeType, Guid? targetId);

        bool IsLoading {  get; }

        void Refresh();

        void LoadIfEmpty();

        void LoadIfNotLoaded();
    }

}
