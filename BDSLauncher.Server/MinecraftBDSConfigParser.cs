using IniParser;
using IniParser.Model;
using IniParser.Parser;

namespace BDSLauncher {
    public class MinecraftBDSConfig {
        public string ServerName { get; set; } = "";
        public string ServerPort4 { get; set; } = "";
        public string ServerPort6 { get; set;} = "";
    }
    public class MinecraftBDSConfigParser {
        private string _BDSExeLocation;
        const string serverProperties = "server.properties";
        const string serverName = "server-name";
        const string ipv4port = "server-port";
        const string ipv6port = "server-portv6";

        public MinecraftBDSConfigParser(string BDSExeLocation) {
            _BDSExeLocation = BDSExeLocation;
        }

        public MinecraftBDSConfig parseConfig () {
            FileInfo inf = new FileInfo(_BDSExeLocation);
            if (inf == null) {
                throw new FileNotFoundException();
            }
            FileInfo? configfile = inf.Directory?.GetFiles(serverProperties).ToList().Single();
            if (configfile == null) {
                throw new FileNotFoundException();
            }

            IniDataParser parser = new IniDataParser();
            parser.Configuration.AllowKeysWithoutSection = true;
            parser.Configuration.CommentString = "#";
            FileIniDataParser fp = new FileIniDataParser(parser);

            IniData data = fp.ReadFile(configfile.FullName);
            return new MinecraftBDSConfig() {
                ServerName = data.GetKey(serverName),
                ServerPort4 = data.GetKey(ipv4port),
                ServerPort6 = data.GetKey(ipv6port),
            };
        }
    }
}