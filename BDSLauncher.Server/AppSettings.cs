namespace BDSLauncher.Server {
    public class AppSettings
    {
        public List<ServerConfig> ServerConfig { get; set; }
    }
    public class ServerConfig
    {
        public string ServerName { get; set; }

        public string ServerPort4 { get; set; }

        public string ServerPort6 { get; set; }
        public string BedrockServerExeLocation { get; set; }
        public string BackupFolderName { get; set; }
        public bool Primary { get; set; }
        public int WCFPortNumber { get; set; }
    }
}