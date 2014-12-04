using Nitrate.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate
{
    public class PluginManager
    {
        [ImportMany]
        public IEnumerable<IPlugin> Plugins { get; set; }

        public Dictionary<string, PluginConfigurations> GetDefaultSettings()
        {
            var configuration = new Dictionary<string, PluginConfigurations>();

            foreach (var plugin in Plugins)
            {
                configuration.Add(plugin.Name, plugin.SampleSettings);
            }

            return configuration;
        }

        public void Configure(Dictionary<string, PluginConfigurations> configuration)
        {
            var errors = new List<string>();

            foreach (var pair in configuration)
            {
                var plugin = Plugins.FirstOrDefault(p => p.Name.Equals(pair.Key, StringComparison.InvariantCultureIgnoreCase));

                if (plugin == null)
                {
                    errors.Add("Unknown plugin: " + pair.Key);
                    continue;
                }

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
                errors.ForEach(e => Shell.Error(e));
            }
        }

        public void Run(string commandName, string[] args)
        {
            if (commandName == "help")
            {
                // Help Command
                if (args.Length == 0 || args.Length > 1)
                {
                    // General help
                    Shell.Info("Usage: no3 <command> <options>");
                    Shell.Lf();
                    Shell.Info("Available Commands:");
                    foreach (var plugin in Plugins)
                    {
                        Shell.Info(" - " + plugin.Name + ": " + plugin.ShortHelp);
                    }
                    Shell.Lf();
                }
                else
                {
                    var plugin = Plugins.FirstOrDefault(p => p.Name.Equals(args[0], StringComparison.InvariantCultureIgnoreCase));

                    // Command specific help
                    if (plugin == null)
                        Shell.Error(args[0] + " is not a command.");
                    else
                    {
                        Shell.Info("Usage: no3 " + args[0] + " <options>");
                        Shell.Lf();
                        Shell.Info(plugin.LongHelp);
                    }
                }
            }
            else
            {
                var plugin = Plugins.FirstOrDefault(p => p.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));
                if (plugin != null)
                {
                    // plugin command
                    if (!plugin.IsAvailable())
                    {
                        Shell.Error("Unavailable plugin: " + plugin.Name);
                        Shell.Error(plugin.InstallationInstructions);
                        return;
                    }

                    var subCommand = String.Empty;
                    if (plugin.SubCommands != null && plugin.SubCommands.Length > 0)
                    {
                        if (args.Length == 0)
                        {
                            Shell.Error("Subcommand missing.");
                            Shell.Info("Run \"no3 help " + commandName + "\" for help.");
                            return;
                        }

                        if (plugin.SubCommands.Contains(args[0]))
                        {
                            subCommand = args[0];
                            args = args.Skip(1).ToArray();
                        }
                        else
                        {
                            Shell.Error("Invalid subcommand \"" + args[0] + "\".");
                            Shell.Info("Run \"no3 help " + commandName + "\" for help.");
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
}
