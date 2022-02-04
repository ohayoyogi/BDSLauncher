using BDSLauncher.Server;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<IBDSManager, BDSManager>();
        services.AddHostedService<BDSConnectorWorker>();
    })
    .Build();

await host.RunAsync();
