using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace Nitrate.Plugins.Git
{
    [Export(typeof(IPlugin))]
    public class Git : BasePlugin
    {
        private const string Program = "git.exe";

        public override string InstallationInstructions
        {
            get { return "Please install git and make sure it's in your PATH."; }
        }

        public override string ShortHelp
        {
            get { return "Manages Git repositories."; }
        }

        public override string LongHelp
        {
            get
            {
                return @"Available subcommands:
 - clone:  clones the repository locally.
 - update: updates the local repository.";
            }
        }

        public override string[] SubCommands
        {
            get { return new string[] { "clone", "update" }; }
        }

        public override PluginConfigurations SampleSettings
        {
            get
            {
                return new PluginConfigurations() {
                    {
                      "Orchard",
                      new PluginConfiguration() {
                        { "Repo", "https://git01.codeplex.com/orchard" },
                        { "Branch", "master" },
                        { "Path", "orchard" }
                      }
                    }
                };
            }
        }

        public override void Run(string subCommand, string configName, string[] args)
        {
            List<string> configs;
            if (configName == "all" || String.IsNullOrWhiteSpace(configName))
                configs = new List<string>(Configuration.Keys.ToArray());
            else
                configs = new List<string>() { configName };
            configs.ForEach(c =>
            {
                switch (subCommand)
                {
                    case "clone":
                        Shell.Info("Cloning " + c + "...");
                        var errorCode = Shell.Run("git.exe", "clone " + Configuration[c]["Repo"] + " " + Configuration[c]["Path"], ProcessOutput.Error, Config.Current.Path);

                        Shell.Run("git.exe", "checkout " + Configuration[c]["Branch"], ProcessOutput.Error, Config.Current.Path);
                        Shell.Success("Done!");
                        break;
                    case "update":
                        break;
                }
            });
        }

        public override bool IsAvailable()
        {
            return Shell.IsAvailable(Program);
        }
    }
}
