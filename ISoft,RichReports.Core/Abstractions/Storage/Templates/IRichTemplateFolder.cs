using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ISoft.RichReports.Core.Abstractions.Storage.Templates
{
    /// <summary>
    /// Represents a template folder in the file system
    /// </summary>
    public interface IRichTemplateFolder
    {
        bool Exists { get; }

        int FileCount { get; }

        string FullPath { get; }

        bool IsLoaded { get; }

        DateTime LastLoadTime { get; }

        string Name { get; }

        string ReportId { get; }

        IEnumerable<IRichTemplateFile> Files { get; }

        IRichTemplateFile AddFile(string fileName);

        Task<IRichTemplateFile> AddFileAsync(string fileName, CancellationToken cancellationToken = default);

        Task<bool> DeleteFileAsync(string fileName, CancellationToken cancellationToken = default);

        void EnsureExists();

        Task EnsureExistsAsync();

        void EnsureLoaded();

        Task EnsureLoadedAsync(CancellationToken cancellationToken = default);

        IRichTemplateFile GetFile(string fileName);

        Task<IRichTemplateFile> GetFileAsync(string fileName, CancellationToken cancellationToken = default);

        Task<int> GetFileCountAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<IRichTemplateFile>> GetFilesAsync(CancellationToken cancellationToken = default);

        IEnumerable<IRichTemplateFile> GetFiles();

        bool HasFile(string fileName);

        Task<bool> HasFileAsync(string fileName, CancellationToken cancellationToken = default);

        void InvalidateCache();

        Task<IEnumerable<IRichTemplateFile>> LoadAllTemplateFilesAsync(CancellationToken cancellationToken = default);

        Task ReloadAsync(CancellationToken cancellationToken = default);

        Task<bool> RenameFileAsync(string oldFileName, string newFileName, CancellationToken cancellationToken = default);

        string ToString();
    }
}
