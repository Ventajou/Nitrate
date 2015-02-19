using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;

namespace Nitrate.Plugins.Git
{
    public class GitConfig
    {
        public string Repo { get; set; }
        public string Branch { get; set; }
        public string Path { get; set; }
        public bool SingleBranch { get; set; }
    }

    [Export(typeof(IPlugin))]
    public class Git : BasePlugin<GitConfig>
    {
        private const string Program = "git.exe";
        private static class Commands
        {
            public const string Clone = "clone";
            public const string Update = "update";
        }

        public override string InstallationInstructions
        {
            get { return "Please install git and make sure it's in your PATH."; }
        }

        public override string Description
        {
            get { return "Manages Git repositories."; }
        }

        private IDictionary<string, SubCommand> _subCommands;
        public override IDictionary<string, SubCommand> SubCommands
        {
            get
            {
                if (_subCommands == null)
                {
                    _subCommands = new Dictionary<string, SubCommand>() { 
                        { Commands.Clone, new SubCommand() { Description = "clones the repository locally" } },
                        { Commands.Update, new SubCommand() { Description = "updates the local repository" } }
                    };
                }
                return _subCommands;
            }
        }

        public override bool IsAvailable()
        {
            return Shell.IsAvailable(Program);
        }

        public override void Execute(string configName, GitConfig config, string subCommand, Dictionary<string, string> args)
        {
            int errorCode;

            switch (subCommand)
            {
                case Commands.Clone:
                    Shell.Info("Cloning " + configName + "...");

                    errorCode = Shell.Run("git.exe", 
                                              String.Format("clone -b {0} {1} \"{2}\" {3}",
                                                config.Branch,
                                                config.Repo,
                                                config.Path,
                                                config.SingleBranch ? " --single-branch" : string.Empty),
                                              ProcessOutput.Window, 
                                              Config.Current.Path);

                    if (errorCode == 0)
                    {
                        Shell.Success("Done!");
                    }
                    else
                    {
                        Shell.Error("An error has occurred.");
                    }
                    break;

                case Commands.Update:
                    Shell.Info("Updating " + configName + "...");

                    errorCode = Shell.Run("git.exe", 
                                          "pull", 
                                          ProcessOutput.All, 
                                          Path.Combine(Config.Current.Path, config.Path));

                    if (errorCode == 0)
                    {
                        Shell.Success("Done!");
                    }
                    else
                    {
                        Shell.Error("An error has occurred.");
                    }
                    break;
            }
        }

        protected override Dictionary<string, GitConfig> SampleConfiguration()
        {
            return new Dictionary<string, GitConfig>()
            {
                {
                    "Orchard", new GitConfig
                    {
                        Repo = "https://git01.codeplex.com/orchard",
                        Branch = "master",
                        Path = "orchard",
                        SingleBranch = true
                    }
                }
            };
        }
    }
}

