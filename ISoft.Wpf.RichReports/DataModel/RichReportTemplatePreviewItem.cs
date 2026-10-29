using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using ISoft.Mvvm.Helpers;
using ISoft.Printing.Core.Xpf.BandDesigner;
using ISoft.Utils.Data.AsyncData;
using ISoft.Utils.DataModel;
using ISoft.Utils.Helpers;
using ISoft.Wpf.RichReports.Helpers;
using System;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISoft.Wpf.RichReports.DataModel
{
    public class XpfRichReportTemplatePreviewItem : POCOMvvmObjectBase, IXpfRichReportTemplatePreviewItem
    {
        private ImageSource _preview;

        #region Proprties
        public Guid? Id { get; set; }

        public string Name { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public byte[] ThumbinalData { get; set; }

        public byte[] Content { get; set; }

        public bool? IsShared { get; set; }

        public Guid? OwnerId { get; set; }

        public string OwnerName { get; set; }

        public virtual bool IsLoading { get; set; }

        public ImageSource PreviewImage
        {
            get
            {
                if (_preview == null)
                {
                    LoadPreview();
                }

                return _preview;
            }
        }

        #endregion

        public XpfRichReportTemplatePreviewItem()
        {

        }

        [Command(false)]
        public void RefreshPreview()
        {
            LoadPreview();
        }

        [Command(false)]
        public void ClearPreview()
        {
            _preview = null;
            this.RaisePropertyChanged(x => x.PreviewImage);
        }

        private void LoadPreview()
        {
            if (IsLoading) return;

            if (Id.IsNullOrEmpty())
            {
                return;
            }

            ReportTemplatesPreviewLoader provider = ReportTemplatesPreviewLoader.Current;
            provider.Load(CreateLoadTask(Id));
        }

        private void OnError(Exception ex)
        {
            NotificationManagerHelper
                .ShowExceptionError($"Error loading template preview", "see log for more info", ex);
        }

        private AsyncTask CreateLoadTask(Guid? templateKey)
        {
            return AsyncTask.Create(
                   token: templateKey,
                   state: templateKey,
                    onStart: () =>
                    {
                        IsLoading = true;
                    },
                   asyncJob: async (token, state) =>
                   {
                       return await Task.Run(() => ImageThumbnailHelper.CreateThumbnail(ThumbinalData, 150, 150));
                   },
                   syncJob: (result) =>
                   {
                       result.Match(
                                failure => OnError(failure),
                                success =>
                                {
                                    AssignPreviewImage(success);
                                });
                   },
                   onFinish: () =>
                   {
                       IsLoading = false;
                   }
               );
        }

        private void AssignPreviewImage(BitmapImage image)
        {
            _preview = image;
            this.RaisePropertyChanged(x => x.PreviewImage);
        }
    }
}
