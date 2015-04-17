using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nitrate
{
	public class ConfigDictionary<T> : Dictionary<string, T>
	{
		public ConfigDictionary()
			: base(StringComparer.OrdinalIgnoreCase)
		{ }

		public ConfigDictionary(IDictionary<string, T> initializer)
			: base(StringComparer.OrdinalIgnoreCase)
		{
			foreach (var item in initializer)
			{
				Add(item.Key, item.Value);
			}
		}
	}

	public class ConfigData
	{
		public string Name { get; set; }
		public string Version { get; set; }
		public ConfigDictionary<string> Globals { get; set; }
		public ConfigDictionary<ConfigDictionary<JObject>> PluginConfigurations { get; set; }
	}

	public class Config
	{
		const string ConfigName = "nitrate.json";
		const string LocalConfigName = "nitrate.local.json";

		private static Config _singleton;
		private string _path;
		private string _workfilesPath;

		public string Path { get { return _path; } }
		public ConfigData Data { get; set; }
		public string WorkfilesPath
		{
			get
			{
				if (String.IsNullOrWhiteSpace(_workfilesPath))
				{
					_workfilesPath = System.IO.Path.Combine(_path, ".nitrate");
					if (!Directory.Exists(_workfilesPath)) Directory.CreateDirectory(_workfilesPath);
				}
				return _workfilesPath;
			}
		}

		public static Config Current { get { return _singleton; } }

		private Config(string path)
		{
			_singleton = this;
			_path = path;
			var file = System.IO.Path.Combine(path, ConfigName);

			if (File.Exists(file))
			{
				var config = JObject.Parse(File.ReadAllText(file));
				var localConfigFile = System.IO.Path.Combine(path, LocalConfigName);
				if (File.Exists(localConfigFile))
				{
					var localConfig = JObject.Parse(File.ReadAllText(localConfigFile));
					config.Merge(localConfig, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });
				}

				Data = config.ToObject<ConfigData>();
			}
		}

		public void Save()
		{
			File.WriteAllText(_path, JsonConvert.SerializeObject(Data, Formatting.Indented));
		}

		public static Config Init(string path, Dictionary<string, Dictionary<string, JObject>> samplePluginConfigurations)
		{
			var pluginConfig = new ConfigDictionary<ConfigDictionary<JObject>>();
			foreach (var pair in samplePluginConfigurations)
			{
				pluginConfig.Add(pair.Key, new ConfigDictionary<JObject>(pair.Value));
			}

			var config = new Config(System.IO.Path.Combine(path, ConfigName));
			config.Data = new ConfigData()
			{
				Name = "Orchard Project",
				Version = "0.0.0",
				PluginConfigurations = pluginConfig
			};
			return config;
		}

		public static Config Load(string path)
		{
			DirectoryInfo info;

			do
			{
				if (File.Exists(System.IO.Path.Combine(path, ConfigName)))
				{
					Shell.Info("Found Nitrate project in: " + path);
					Shell.Lf();
					return new Config(path);
				}

				info = Directory.GetParent(path);
				path = (info == null) ? "" : info.FullName;
			}
			while (info != null);

			return null;
		}
	}
}