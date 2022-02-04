using System.Diagnostics;

namespace BDSLauncher.Server;
public class BDSManager : IBDSManager
{
    AppSettings _appSettings;
    Dictionary<int, Process> _processes;
    public BDSManager(IConfiguration configuration)
    {
        this._processes = new Dictionary<int, Process>();
        this._appSettings = configuration.GetSection("appSettings").Get<AppSettings>();

        var usedPort = new List<int>();
        foreach (var conf in _appSettings.ServerConfig)
        {
            var config = new MinecraftBDSConfigParser(conf.BedrockServerExeLocation).parseConfig();
            int port4 = Int32.Parse(config.ServerPort4);
            int port6 = Int32.Parse(config.ServerPort6);

            if (usedPort.Contains(port4))
            {
                throw new Exception($"Port {port4} is already in used");
            }
            usedPort.Add(port4);
            if (usedPort.Contains(port6))
            {
                throw new Exception($"Port {port6} is already in used");
            }
            usedPort.Add(port6);
        }

        for (int idx = 0; idx < _appSettings.ServerConfig.Count; idx++)
        {
            var serverConfig = _appSettings.ServerConfig[idx];
            var _process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    // RedirectStandardError = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    // WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = serverConfig.BedrockServerExeLocation,
                },
                EnableRaisingEvents = true,
            };
            _process.Start();
            _process.BeginOutputReadLine();
            _process.PriorityClass = ProcessPriorityClass.RealTime;
            this._processes.Add(idx, _process);
        }
    }

    public String[] getServerNames()
    {
        return this._appSettings.ServerConfig.Select(item => item.ServerName).ToArray();
    }
    public String SendCommand(int target, String command)
    {
        var _process = this._processes[target];
        _process.StandardInput.WriteLine(command);
        return $"target: {target}, command: {command}, exited= {_process.HasExited}";
    }

    public void Subscribe(int target, IBDSManager.OnUpdateDelegate onUpdated)
    {
        Console.WriteLine("subscribe: " + target);
        var _process = this._processes[target];
        _process.OutputDataReceived += (sender, e) =>
        {
            onUpdated(e.Data ?? "");
        };
    }
}
