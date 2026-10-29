using ISoft.RichReports.Core.Abstractions.Templates;
using ISoft.RichReports.Core.Models.Templates;
using System;

namespace ISoft.RichReports.Core.Templates
{
    public class RichReportTemplate : ObjectBase, IRichReportTemplate
    {
        public const string FileExtension = ".rrt";

        public const string FileFilter = $"Rich Report Template Files|*{FileExtension}";

        public const string CurrentVersion = "1";

        #region Fields
        private Guid? _id;
        private string _name;
        private byte[] _content;
        private byte[] thumbnail;
        private string _version = CurrentVersion;
        #endregion

        #region Proprties
        public Guid? Id
        {
            get => _id;
            set { _id = value; }
        }
       
        public string Name
        {
            get => _name;
            set { _name = value; }
        }

        public byte[] Content
        {
            get => _content;
            set { _content = value; }
        }

        public byte[] Thumbnail 
        {
            get => thumbnail; 
            set => thumbnail = value; 
        }

        public string Version
        {
            get => _version; 
            set => _version = value;
        }

        #endregion

        #region Actions
        void IRichReportTemplate.UpdateContent(byte[] data)
        {
            Content = data;
        }

        void IRichReportTemplate.UpdateThumbnail(byte[] data)
        {
            Thumbnail = data;
        }

        IRichReportTemplate IRichReportTemplate.CreateCopy(Guid id)
        {
            var clone = this.CreateClone();
            clone.Id = id;
            return clone;
        }

        IRichReportTemplate IRichReportTemplate.CreateCopy(Guid id, string name)
        {
            var clone = this.CreateClone();
            clone.Id = id;
            clone.Name = name;
            return clone;
        }

        public string GetDisplayName() => $"( {Name} )";

        public string GetDesignerTitle() => $"تصميم - {GetDisplayName()}";

        public string GetPreviewTitle() => $"معاينة - {GetDisplayName()}";

        public RichReportTemplateData CreateTemplateData()
        {
            return CreateTemplateData(this);
        }

        public static RichReportTemplate CreateFromData(IRichReportTemplateData info)
        {
            return new RichReportTemplate
            {
                Id = info.Id,
                Name = info.Name,
                Content = info.Content,
                Thumbnail = info.Thumbnail,
                Version = info.Version ?? CurrentVersion
            };
        }

        public static RichReportTemplateData CreateTemplateData(RichReportTemplate template)
        {
            return new RichReportTemplateData
            {
                Id = template.Id,
                Name = template.Name,
                Content = template.Content,
                Thumbnail = template.Thumbnail,
                Version = template.Version,
            };
        }
        #endregion
    }
}
