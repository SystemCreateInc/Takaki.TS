using LogLib;
using Microsoft.Extensions.Hosting;

namespace ProcessorLib
{
    public class ServiceWorker<COMMAND_PROCESSOR> 
        : BackgroundService 
        where COMMAND_PROCESSOR : ICommandProcessor
    {
        public COMMAND_PROCESSOR Processor { get; init; }

        private readonly string _capability;
        private readonly TaskTrayIcon _trayIcon;

        public ServiceWorker(string capability, Icon icon, COMMAND_PROCESSOR processor, IHostApplicationLifetime hostAppLifetime)
        {
            Processor = processor;
            _capability = capability;
            _trayIcon = new TaskTrayIcon(hostAppLifetime, icon);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Syslog.Info($"Started");
            var handler = new BackendMessageProcessorHandler(new ProcessorAdapter<COMMAND_PROCESSOR>(_capability, Processor));
            await Task.Run(() => handler.Run(stoppingToken), stoppingToken);
            Syslog.Info($"Stopped");
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _trayIcon.Show();
            return base.StartAsync(cancellationToken);
        }
    }
}
