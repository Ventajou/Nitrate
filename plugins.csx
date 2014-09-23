#load "Plugins\base.csx"
#load "Plugins\git.csx"

using System.Linq;

public static class Plugins
{
  private static Dictionary<string, BasePlugin> _plugins = new Dictionary<string, BasePlugin>(StringComparer.OrdinalIgnoreCase);

  public static void Register(BasePlugin plugin)
  {
    _plugins.Add(plugin.Name, plugin);
  }

  public static Dictionary<string, PluginSettings> GetDefaultSettings()
  {
    var configuration = new Dictionary<string, PluginSettings>();

    foreach(var pair in _plugins)
    {
      if (pair.Value.IsAvailable())
      {
        configuration.Add(pair.Key, pair.Value.DefaultSettings);
      }
    }

    return configuration;
  }

  public static void Configure(Dictionary<string, PluginSettings> configuration)
  {
    var errors = new List<string>();

    foreach(var pair in configuration)
    {
      if (!_plugins.ContainsKey(pair.Key))
      {
        errors.Add("Unknown plugin: " + pair.Key);
        continue;
      }

      var plugin = _plugins[pair.Key];

      if (!plugin.IsAvailable())
      {
        errors.Add("Unavailable plugin: " + pair.Key);
        errors.Add(plugin.InstallationInstructions);
        continue;
      }

      plugin.Configure(pair.Value);
    }

    if (errors.Count() > 0)
    {
      errors.ForEach(e => Con.Error(e));
    }
  }

  public static void Run(string commandName, string[] args)
  {
    if (commandName == "help")
    {
      // Help Command
      if (args.Length == 0 || args.Length > 1)
      {
        // General help
        Con.Info("Usage: no3 <command> <options>");
        Con.Lf();
        Con.Info("Available Commands:");
        foreach(var pair in _plugins)
        {
          Con.Info(" - " + pair.Key + ": " + pair.Value.ShortHelp);
        }
        Con.Lf();
      }
      else
      {
        // Command specific help
        if (!_plugins.ContainsKey(args[0]))
          Con.Error(args[0] + " is not a command.");
        else
        {
          Con.Info("Usage: no3 " + args[0] + " <options>");
          Con.Lf();
          Con.Info(_plugins[args[0]].LongHelp);
        }
      }
    }
    else
    {
      if (_plugins.ContainsKey(commandName))
      {
        // plugin command
        var plugin = _plugins[commandName];
        if (!plugin.IsAvailable())
        {
          Con.Error("Unavailable plugin: " + plugin.Name);
          Con.Error(plugin.InstallationInstructions);
          return;
        }

        var subCommand = String.Empty;
        if (plugin.SubCommands != null && plugin.SubCommands.Length > 0)
        {
          if (args.Length == 0)
          {
            Con.Error("Subcommand missing.");
            Con.Info("Run \"no3 help " + commandName + "\" for help.");
            return;
          }

          if (plugin.SubCommands.Contains(args[0]))
          {
            subCommand = args[0];
            args = args.Skip(1).ToArray();
          }
          else
          {
            Con.Error("Invalid subcommand \"" + args[0] + "\".");
            Con.Info("Run \"no3 help " + commandName + "\" for help.");
            return;
          }
        }

        var configName = String.Empty;
        if (args.Length > 0)
        {
          if (args[0] == "all" || plugin.Configuration.ContainsKey(args[0]))
          {
            configName = args[0];
            args = args.Skip(1).ToArray();
          }
        }

        plugin.Run(subCommand, configName, args);
      }
    }
  }
}
