using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.Editor.Services.Abstractions
{
    public interface IRichWaitIndicationService
    {
        Task BeginAsync(string state);

        Task UpdateAsync(string state);

        Task<AsyncExecutionResult> ShowWaitIndicatorAsync(Func<Task> asyncAction, string state = null, bool throwExceptions = true);

        Task EndAsync();
    }
}
