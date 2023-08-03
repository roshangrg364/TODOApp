using DomainModule.ServiceInterface;
using Microsoft.AspNetCore.SignalR;
using TodoApp.SignalR;

namespace TodoApp.CronJob
{
    public class NotifyCronJob : CronJobService
    {
        private readonly IHubContext<TodoHub> _hub;
        public NotifyCronJob(IScheduleConfig<NotifyCronJob> config, IHubContext<TodoHub> hub, IHttpContextAccessor httpContextAccessor)
        : base(config.CronExpression, config.TimeZoneInfo)
        {
            _hub = hub;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {

            return base.StartAsync(cancellationToken);
        }

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            await _hub.Clients.All.SendAsync("ShowRemainder").ConfigureAwait(false);
            await Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {

            return base.StopAsync(cancellationToken);
        }
    }

}