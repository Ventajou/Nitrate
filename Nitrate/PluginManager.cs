using Newtonsoft.Json.Linq;
using Nitrate.Plugins;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nitrate
{
	public class PluginManager
	{
		[ImportMany]
		public IEnumerable<IPlugin> Plugins { get; set; }

		public Dictionary<string, Dictionary<string, JObject>> GetSampleConfiguration()
		{
			var configuration = new Dictionary<string, Dictionary<string, JObject>>();

			foreach (var plugin in Plugins)
			{
				configuration.Add(plugin.Name, plugin.GetSampleConfiguration());
			}

			return configuration;
		}

		public void Run(string commandName, string[] args)
		{
			if (commandName == "help")
			{
				// Help Command
				if (args.Length == 0 || args.Length > 1)
				{
					// General help
					Shell.Write("Usage: no3 <command[:configuration]> <options>");
					Shell.Lf();
					Shell.Write("Available Commands:");
					foreach (var plugin in Plugins)
					{
						Shell.Write(" - " + plugin.Name + ": " + plugin.Description);
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
						Shell.Write("Usage: no3 " + args[0] + " <options>");
						Shell.Lf();
						Shell.Write(plugin.Description);
						if (plugin.SubCommands != null)
						{
							Shell.Lf();
							Shell.Write("Available subcommands:");
							foreach (var subCommand in plugin.SubCommands)
							{
								Shell.Lf();
								Shell.Write(String.Format(" - {0}: {1}", subCommand.Key, subCommand.Value.Description));
								if (!string.IsNullOrWhiteSpace(subCommand.Value.Example))
								{
									Shell.Write(String.Format("   example: no3 {0} {1} {2}", args[0], subCommand.Key, subCommand.Value.Example));
								}
								if (subCommand.Value.Arguments != null && subCommand.Value.Arguments.Count > 0)
								{
									Shell.Write("   arguments:");
									foreach (var argument in subCommand.Value.Arguments)
									{
										Shell.Write(string.Format("   - {0}: {1}", argument.Name, argument.Description));
									}
								}
							}
						}
					}
				}
			}
			else
			{
				var configName = String.Empty;
				var commandParts = commandName.Split(':');
				switch (commandParts.Length)
				{
					case 1:
						break;
					case 2:
						commandName = commandParts[0];
						configName = commandParts[1];
						break;
					default:
						break;
				}

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
					var parsedArguments = new Dictionary<string, string>();
					if (plugin.SubCommands != null)
					{
						if (args.Length == 0)
						{
							Shell.Error("Subcommand missing.");
							Shell.Write("Run \"no3 help " + commandName + "\" for help.");
							return;
						}

						if (plugin.SubCommands.Keys.Contains(args[0]))
						{
							subCommand = args[0];
							args = args.Skip(1).ToArray();

							if (plugin.SubCommands[subCommand].Arguments != null)
							{
								var clone = new List<Argument>(plugin.SubCommands[subCommand].Arguments);

								foreach (var arg in args)
								{
									var argInfo = clone.FirstOrDefault(c => Regex.IsMatch(arg, c.Regex));
									if (argInfo == null)
									{
										Shell.Error("Invalid argument \"" + arg + "\".");
										Shell.Write("Run \"no3 help " + commandName + "\" for help.");
										return;
									}

									clone.Remove(argInfo);
									parsedArguments.Add(argInfo.Name, arg);
								}
							}
							else if (args.Length > 0)
							{
								Shell.Error("This command does not take arguments.");
								return;
							}

						}
						else
						{
							Shell.Error("Invalid subcommand \"" + args[0] + "\".");
							Shell.Write("Run \"no3 help " + commandName + "\" for help.");
							return;
						}
					}

					if (string.IsNullOrWhiteSpace(configName))
					{
						foreach (var config in Config.Current.Data.PluginConfigurations[plugin.Name])
						{
							plugin.Execute(config.Key, config.Value, subCommand, parsedArguments);
							Shell.Lf();
						}
					}
					else
					{
						plugin.Execute(configName, Config.Current.Data.PluginConfigurations[plugin.Name][configName], subCommand, parsedArguments);
					}
				}
				else
				{
					Shell.Error(commandName + " is not a command.");
					Shell.Write("Run no3 help for a list of available commands.");
				}
			}
		}
	}
}
