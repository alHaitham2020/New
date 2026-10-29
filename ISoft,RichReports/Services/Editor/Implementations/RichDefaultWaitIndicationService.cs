using System;
using System.Threading.Tasks;

namespace ISoft.RichReports.Services.Editor.Implementations
{
    public class RichDefaultWaitIndicationService : IRichWaitIndicationService
    {
        public RichDefaultWaitIndicationService()
        {

        }

        public Task BeginAsync(string state) => Task.CompletedTask;

        public Task UpdateAsync(string state) => Task.CompletedTask;

        public Task EndAsync() => Task.CompletedTask;

        public Task<AsyncExecutionResult> ShowWaitIndicatorAsync(Func<Task> asyncAction, string state = null, bool throwExceptions = true)
        {
            return Task.FromResult(AsyncExecutionResult.AsSuccess());
        }
    }
}
