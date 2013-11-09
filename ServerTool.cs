using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAsylum.BattleLog
{
    public static class ServerTool
    {
        public static Entry[] GetRange(long offset, long count)
        {
            var request = WebRequest.CreateHttp(string.Format("http://battlelog.battlefield.com/bf4/servers/getServers/pc/?offset={0}&count={1}", offset, count));
            request.Headers["Accept-Encoding"] = "gzip";
            request.Host = "battlelog.battlefield.com";
            request.Headers["Accept-Language"] = "en-US,en;q=0.8";
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
            request.Accept = "application/json";
            request.Headers["X-Requested-With"] = "XMLHttpRequest";
            using(var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                byte[] result;
                using (var gz = new GZipStream(stream, CompressionMode.Decompress))
                {
                    const int size = 4096;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int gcount = 0;
                        do
                        {
                            gcount = gz.Read(buffer, 0, size);
                            if (gcount > 0)
                                memory.Write(buffer, 0, gcount);
                        }
                        while (gcount > 0);
                        result = memory.ToArray();
                    }
                }
                var json = Encoding.UTF8.GetString(result);
                return JsonConvert.DeserializeObject<EntryResponse>(json).data;
            }
        }

        public class EntryResponse
        {
            public string type;
            public string message;
            public Entry[] data;
        }

        public class Entry
        {
            public class Comparer : IEqualityComparer<Entry>
            {
                public static readonly Comparer Instance = new Comparer();

                public bool Equals(Entry x, Entry y)
                {
                    return x.guid == y.guid;
                }

                public int GetHashCode(Entry obj)
                {
                    return obj.GetHashCode();
                }
            }

            public string map;
            public string gameId;
            public byte[] gameExpansions;
            public int mapMode;
            public string ip;
            public string matchId;
            public string protocolVersionString;
            public EntryData extendedInfo;
            public int game;
            public bool ranked;
            public bool online;
            public byte platform;
            public int updatedAt;
            public dynamic slots;
            public Guid guid;
            public UInt16 port;
            public bool punkbuster;
            public byte gameExpansion;
            public string name;
            public bool fairfight;
            public byte region;
            public byte mapVariant;
            public byte serverType;
            public byte experience;
            public bool hasPassword;
            public string[] maps;
            public string secret;
            public int preset;
            public string country;

            public override string ToString()
            {
                return name;
            }
        }

        public class EntryData
        {
            public string message;
            public string bannerUrl;
            public string desc;
        }
    }
}
