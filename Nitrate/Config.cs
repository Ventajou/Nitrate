using Newtonsoft.Json;
using Nitrate.Plugins;
using System.Collections.Generic;
using System.IO;

namespace Nitrate
{
    public class ConfigData
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public Dictionary<string, PluginConfigurations> Configuration { get; set; }
    }

    public class Config
    {

        const string ConfigName = "nitrate.json";

        private Config(string path, PluginManager pluginManager)
        {
            _singleton = this;
            _path = path;
            var file = System.IO.Path.Combine(path, ConfigName);

            if (File.Exists(file))
            {
                Data = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(file));
                pluginManager.Configure(Data.Configuration);
            }
        }

        private static Config _singleton;

        private string _path;
        public string Path { get { return _path; } }

        public ConfigData Data { get; set; }

        public static Config Current { get { return _singleton; } }

        public void Save()
        {
            File.WriteAllText(_path, JsonConvert.SerializeObject(Data, Formatting.Indented));
        }

        public static Config Init(string path, PluginManager pluginManager)
        {
            var config = new Config(System.IO.Path.Combine(path, ConfigName), pluginManager);
            config.Data = new ConfigData()
            {
                Name = "Orchard Project",
                Version = "0.0.0",
                Configuration = pluginManager.GetDefaultSettings()
            };
            return config;
        }

        public static Config Load(string path, PluginManager pluginManager)
        {
            DirectoryInfo info;

            do
            {
                if (File.Exists(System.IO.Path.Combine(path, ConfigName)))
                {
                    Shell.Success("Found Nitrate project in: " + path);
                    return new Config(path, pluginManager);
                }

                info = Directory.GetParent(path);
                path = (info == null) ? "" : info.FullName;
            }
            while (info != null);

            return null;
        }
    }
}