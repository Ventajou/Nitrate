using GlobDir;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Nitrate.Plugins.Run
{
	public class SymLinksConfig
	{
		public string Source { get; set; }
		public string Destination { get; set; }
	}

	[Export(typeof(IPlugin))]
	public class SymLinks : BasePlugin<SymLinksConfig>
	{
		[DllImport("kernel32.dll")]
		static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

		private static class Commands
		{
			public const string Create = "create";
			public const string Remove = "remove";
		}

		public override string Description
		{
			get { return "Manages symbolic links."; }
		}

		private IDictionary<string, SubCommand> _subCommands;
		public override IDictionary<string, SubCommand> SubCommands
		{
			get
			{
				if (_subCommands == null)
				{
					_subCommands = new Dictionary<string, SubCommand>() {
						{ Commands.Create, new SubCommand() { Description = "creates symbolic links" } },
						{ Commands.Remove, new SubCommand() { Description = "removes symbolic links" } }
					};
				}
				return _subCommands;
			}
		}

		protected override Dictionary<string, SymLinksConfig> SampleConfiguration()
		{
			return new Dictionary<string, SymLinksConfig>()
			{
				{
					"Modules", new SymLinksConfig
					{
						Source = @"src\modules\*\",
						Destination = @"orchard\src\Orchard.Web\Modules"
					}
				},
				{
					"Themes", new SymLinksConfig
					{
						Source = @"src\themes\*\",
						Destination = @"orchard\src\Orchard.Web\Themes"
					}
				}
			};
		}

		public override bool IsAvailable()
		{
			return true;
		}

		public override void Execute(string configName, SymLinksConfig config, string subCommand, Dictionary<string, string> args)
		{
			var sources = Glob.GetMatches(Path.Combine(Config.Current.Path, config.Source).Replace("\\", "/"));

			switch (subCommand)
			{
				case Commands.Create:
					Shell.Write("Creating symlinks for " + configName + "...");
					if (sources.Count() == 0)
					{
						Shell.Info("No files or directories found.");
					}
					else foreach (var source in sources)
						{
							var attributes = File.GetAttributes(source);
							var dest = Path.Combine(config.Destination, GetTargetName(source));

							if (CreateSymbolicLink(
									dest,
									source,
									(attributes & FileAttributes.Directory) == FileAttributes.Directory ? 1 : 0))
							{
								Shell.Info("Created symbolic link: " + dest);
							}
							else
							{
								Shell.Warn("Could not create link: " + dest);
							}
						}
					break;

				case Commands.Remove:
					Shell.Write("Removing symlinks for " + configName + "...");
					if (sources.Count() == 0)
					{
						Shell.Info("No files or directories found.");
					}
					else foreach (var source in sources)
						{
							var dest = Path.Combine(config.Destination, GetTargetName(source));
							if (File.Exists(dest))
							{
								File.Delete(dest);
							}
							else if (Directory.Exists(dest))
							{
								Directory.Delete(dest);
							}
							Shell.Info("Removed " + dest);
						}
					break;
			}
		}

		private string GetTargetName(string path)
		{
			return path.Split('/').Last(x => !string.IsNullOrWhiteSpace(x));
		}
	}
}
