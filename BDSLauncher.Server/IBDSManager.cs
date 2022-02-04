
namespace BDSLauncher.Server;

public interface IBDSManager
{
    public String[] getServerNames();
    public delegate void OnUpdateDelegate(string value);
    public String SendCommand(int target, String command);
    public void Subscribe(int target, OnUpdateDelegate onUpdated);
}
