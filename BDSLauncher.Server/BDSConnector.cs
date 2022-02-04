
using BDSLauncher.Grpc;
using Grpc.Core;

namespace BDSLauncher.Server;

public class BDSConnector : BDSConn.BDSConnBase
{
    IBDSManager _manager; 

    public BDSConnector(IBDSManager manager) {
        this._manager = manager;
    }

    public override Task<ListServerOutput> ListServer(ListServerInput request, ServerCallContext context)
    {
        var result = new ListServerOutput();
        result.ServerNames.Add(_manager.getServerNames());
        return Task.FromResult(result);
    }

    public override Task<OutgoingMessage> Command(BedrockCommand request, ServerCallContext context)
    {
        var result = _manager.SendCommand(request.Target, request.Command);
        Console.WriteLine(result);
        return Task.FromResult(new OutgoingMessage() { Message = "Command is sent" });
    }

    public override async Task Subscribe(SubscribeTarget request, IServerStreamWriter<OutgoingMessage> responseStream, ServerCallContext context)
    {
        var channel = System.Threading.Channels.Channel.CreateBounded<String>(10);

        _manager.Subscribe(request.Target, async (String value) => {
            while(await channel.Writer.WaitToWriteAsync()) {
                channel.Writer.TryWrite(value);
                break;
            }
        });
        while(!context.CancellationToken.IsCancellationRequested) {
            while(await channel.Reader.WaitToReadAsync()) {
                await foreach(var i in channel.Reader.ReadAllAsync()) {
                    await responseStream.WriteAsync(new OutgoingMessage{ Message = i});
                }
            }
            await Task.Delay(1000, context.CancellationToken);
        }
    }
}