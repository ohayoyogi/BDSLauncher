using BDSLauncher.Grpc;
using Grpc.Core;

using System.ComponentModel;

Channel channel = new Channel("localhost:50051", ChannelCredentials.Insecure);

var client = new BDSConn.BDSConnClient(channel);

var serverNames = client.ListServer(new ListServerInput()).ServerNames;
Console.WriteLine("Select server");
for (int idx = 0; idx < serverNames.Count; idx++)
{
    Console.WriteLine($"{idx} : {serverNames[idx]}");
}

int intTarget = 0;
while (true)
{
    Console.Write("Server Number ? ");
    var line = Console.ReadLine();
    if (Int32.TryParse(line, out intTarget) &&
    0 <= intTarget && intTarget < serverNames.Count) break;
}

BackgroundWorker worker = new BackgroundWorker();
worker.DoWork += async (sender, e) =>
{
    // listen 
    var hoge = client.Subscribe(new SubscribeTarget() { Target = intTarget });
    while (!hoge.ResponseHeadersAsync.IsCanceled)
    {
        var cur = await hoge.ResponseStream.MoveNext();
        Console.WriteLine(hoge.ResponseStream.Current.Message);
    }
};
worker.RunWorkerAsync();

using (var consoleInputStream = Console.OpenStandardInput())
{
    using (var sr = new StreamReader(consoleInputStream))
    {
        while (!sr.EndOfStream)
        {
            var inputLine = await sr.ReadLineAsync();
            client.Command(new BedrockCommand() { Command = inputLine, Target = intTarget });
        }
    }
}
