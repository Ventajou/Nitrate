public class PluginSettings : Dictionary<string, Dictionary<string, string>>
{
  public PluginSettings():base()
  {}

  public PluginSettings(IEqualityComparer<string> comparer): base(comparer)
  {}
}

public abstract class BasePlugin
{
  public string Name { get { return this.GetType().Name; } }

  public abstract string InstallationInstructions { get; }
  public abstract PluginSettings DefaultSettings { get; }
  public abstract string ShortHelp { get; }
  public abstract string LongHelp { get; }
  public abstract string[] SubCommands { get; }

  public PluginSettings Configuration { get; set; }

  public abstract void Run(string subCommand, string configName, string[] args);
  public abstract bool IsAvailable();

  public void Configure(PluginSettings settings)
  {
    var newSettings = new PluginSettings(StringComparer.OrdinalIgnoreCase);
    foreach(var pair in settings)
    {
      newSettings.Add(pair.Key, new Dictionary<string, string>(pair.Value, StringComparer.OrdinalIgnoreCase));
    }
    Configuration = newSettings;
  }
}
