using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mamo233Lib
{
    public static class Env
    {
        private static Dictionary<string, string> Envs = new Dictionary<string, string>();
        public static bool DEBUG => bool.Parse(Envs["DEBUG"]);
        public static string TG_TOKEN => Envs["TG_TOKEN"];
        public static long WORK_GROUP => long.Parse(Envs["WORK_GROUP"]);
        public static string PROXY => Envs["PROXY"];
        public static bool USE_PROXY => bool.Parse(Envs["USE_PROXY"]);
        public static TimeSpan TIMEOUT => TimeStamp.ParseDuration(Envs["TIMEOUT"]);
        public static TimeSpan INTERVAL => TimeStamp.ParseDuration(Envs["INTERVAL"]);
        public static string DB_TYPE => Envs["DB_TYPE"];
        public static string DB_FILE => Envs["DB_FILE"];
        public static string DB_CONNECTION_STRING => Envs["DB_CONNECTION_STRING"];
        static Env()
        {
            LoadEnvironmentVariables(Envs);
            LoadEnvFileVariables(Envs, ".env");
        }
        public static string Get(string key)
        {
            return Envs[key];
        }

        private static void LoadEnvironmentVariables(Dictionary<string, string> configVariables)
        {
            IDictionary environmentVariables = Environment.GetEnvironmentVariables();
            foreach (DictionaryEntry de in environmentVariables)
            {
                try
                {
                    configVariables[de.Key.ToString()!] = de.Value!.ToString()!;
                }
                catch (Exception e)
                {
                    Log.Error(e, $"环境变量无法读取");
                }
            }
        }

        private static void LoadEnvFileVariables(Dictionary<string, string> configVariables, string filePath)
        {
            if (!File.Exists(filePath)) return;

            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line) && line.StartsWith("#")) continue;

                var parts = line.Split('=', 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
                var value = parts[1].Trim();

                if (!configVariables.ContainsKey(key))
                    configVariables[key] = value;
            }
        }
    }
}
