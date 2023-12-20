using LogLib;
using ProcessorLib;
using StowageSvr;
using StowageSvr.Reporitories;
using System.Reflection;

Syslog.Init();

// サービスインストール
if (ServiceInstaller.ParseCommandLine(args, "ScStowageSvr"))
{
    return;
}

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = Assembly.GetEntryAssembly()?.GetName().Name ?? "Service";
    })
    .ConfigureServices(services =>
    {
        services
            .AddTransient<IRepositoryFactory<IStowageRepository>, StowageRepositoryFactory>()
            .AddSingleton<StowageProcessor>()
            .AddTransient(sp => StowageSvr.Properties.Resources.Service)
            .AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();

