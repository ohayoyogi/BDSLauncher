using Grpc.Core;
using GrpcServer = Grpc.Core.Server;
using BDSLauncher.Grpc;

namespace BDSLauncher.Server;
public class BDSConnectorWorker : IHostedService
{
    private GrpcServer? _server;
    private readonly ILogger<BDSConnectorWorker> _logger;

    private readonly IBDSManager _manager;
    // private Worker _worker;

    public BDSConnectorWorker(IBDSManager manager, ILogger<BDSConnectorWorker> logger)
    {
        _logger = logger;
        _manager = manager;
    }

    public Task StartAsync(CancellationToken stoppingtoken) {
        int DefaultPort = 50051;

        _server = new GrpcServer() {
            Services = { BDSConn.BindService(new BDSConnector(_manager) )},
            Ports = { new ServerPort("localhost", DefaultPort, ServerCredentials.Insecure)},
        };

        _server.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken) {
        return _server?.ShutdownAsync() ?? Task.CompletedTask;
    }
}
