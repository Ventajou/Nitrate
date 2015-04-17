using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;

namespace Nitrate.Plugins.Orchard
{
	public class OrchardConfig
	{
		public string Path { get; set; }
		public string SiteName { get; set; }
		public string AdminUser { get; set; }
		public string AdminPassword { get; set; }
		public string DatabaseProvider { get; set; }
		public string ConnectionString { get; set; }
		public string Recipe { get; set; }
	}

	[Export(typeof(IPlugin))]
	class Orchard : BasePlugin<OrchardConfig>
	{
		private static class Commands
		{
			public const string Setup = "setup";
		}

		public override string Description
		{
			get { return "Manages Orchard CMS"; }
		}

		private IDictionary<string, SubCommand> _subCommands;
		public override IDictionary<string, SubCommand> SubCommands
		{
			get
			{
				if (_subCommands == null)
				{
					_subCommands = new Dictionary<string, SubCommand>() {
						{ Commands.Setup, new SubCommand() { Description = "sets up a new Orchard instance" } }
					};
				}
				return _subCommands;
			}
		}

		public override void Execute(string configName, OrchardConfig config, string subCommand, Dictionary<string, string> args)
		{
			var orchardPath = Path.Combine(Config.Current.Path, config.Path);
			if (!File.Exists(orchardPath))
			{
				Shell.Error(String.Format("The Orchard executable was not found at: {0}", config.Path));
				return;
			}

			switch (subCommand)
			{
				case Commands.Setup:
					Shell.Info(string.Format("Setting up Orchard site {0}", config.SiteName));
					var code = Shell.Run(orchardPath, string.Format("setup /sitename:{0} /adminusername:{1} /adminpassword:{2} /databaseprovider:{3} /DatabaseConnectionString:\"{4}\" /recipe:{5}",
							config.SiteName,
							config.AdminUser,
							config.AdminPassword,
							config.DatabaseProvider,
							config.ConnectionString,
							config.Recipe),
						ProcessOutput.All);
					if (code != 0)
					{
						Shell.Error("Could not create Orchard site. Does it already exist?");
						return;
					}
					Shell.Run(orchardPath, "feature enable Orchard.CodeGeneration", ProcessOutput.All);
					Shell.Success("Done.");
					break;
			}
		}

		protected override Dictionary<string, OrchardConfig> SampleConfiguration()
		{
			return new Dictionary<string, OrchardConfig>()
			{
				{
					"Orchard", new OrchardConfig
					{
						Path=@"orchard\src\orchard.web\bin\orchard.exe",
						SiteName="Orchard",
						AdminUser="admin",
						AdminPassword="password",
						DatabaseProvider="SQLServer",
						ConnectionString="Data Source=localhost;Initial Catalog=orchard;User Id=orchard_user; Password=ocd_1234;",
						Recipe="Default"
					}
				}
			};
		}
	}
}
