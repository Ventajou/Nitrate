using Newtonsoft.Json;
using System.IO;

public class ConfigData {
  public ConfigData()
  {
    Name = "Nitrate project";
    Version = "0.0.0";
  }

  public string Name { get; set; }
  public string Version { get; set; }
}

public class Config {

  const string ConfigName = "nitrate.json";

  private Config(string path)
  {
    _path = path;

    if (File.Exists(path))
      Data = JsonConvert.DeserializeObject<ConfigData>(File.ReadAllText(path));
    else
      Data = new ConfigData();
  }

  private string _path;

  public ConfigData Data { get; set; }

  public void Save()
  {
    File.WriteAllText(_path, JsonConvert.SerializeObject(Data));
  }

  public static Config Init()
  {
    return new Config(Path.Combine(Directory.GetCurrentDirectory(), ConfigName));
  }

  public static Config Load()
  {
    var path = Directory.GetCurrentDirectory();
    DirectoryInfo info;

    do {
      if (File.Exists(Path.Combine(path, ConfigName)))
      {
        return new Config(path);
      }

      info = Directory.GetParent(path);
      path = (info == null) ? "" : info.FullName;
    }
    while(info != null);

    return null;
  }
}
