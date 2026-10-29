namespace ISoft.Wpf.RichReports.Services
{
    public class RichWaitIndicationService : IRichWaitIndicationService
    {
        private readonly ISplashScreenManagerServicePlus _ssms;
        private readonly TimeSpan _delay;

        public RichWaitIndicationService(ISplashScreenManagerServicePlus ssms = null, TimeSpan? delay = null)
        {
            _ssms = ssms ?? DomainServiceContainerHelper.GetService<ISplashScreenManagerServicePlus>();
            _delay = delay ?? TimeSpan.Zero;
        }

        public async Task BeginAsync(string state) => await _ssms.ShowWaitIndicatorAsync(state, _delay);

        public async Task UpdateAsync(string state) => await _ssms.UpdateWaitIndicatorAsync(state, _delay);

        public async Task EndAsync() => await _ssms.CloseAsync(_delay);

        public async Task<AsyncExecutionResult> ShowWaitIndicatorAsync(Func<Task> asyncAction, string state = null, bool throwExceptions = true)
        {
           return await _ssms.ExecuteWithWaitIndicatorAsync(asyncAction, state, throwExceptions);
        }
    }
}
