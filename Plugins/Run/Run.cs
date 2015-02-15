using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Nitrate.Plugins.Run
{
    public class RunConfig
    {
        public string[] Commands { get; set; }
    }

    [Export(typeof(IPlugin))]
    public class Run : BasePlugin<RunConfig>
    {
        public override string Description
        {
            get { return "Runs a set of commands as a single one."; }
        }

        protected override Dictionary<string, RunConfig> SampleConfiguration()
        {
            return new Dictionary<string, RunConfig>()
            {
                {
                    "Setup", new RunConfig
                    {
                        Commands = new string[] {
                            "git clone",
                            "msbuild",
                            "sqlserver create -f"
                        }                    
                    }
                }

            };
        }

        public override bool IsAvailable()
        {
            return true;
        }

        public override void Execute(string configName, RunConfig config, string subCommand, Dictionary<string, string> args)
        {
            Shell.Write("Running " + configName + "...");
            foreach(var command in config.Commands)
            {
                var cmd = command.Split(' ');
                Program.PluginManager.Run(cmd.First(), cmd.Skip(1).ToArray());
            }
        }
    }
}
