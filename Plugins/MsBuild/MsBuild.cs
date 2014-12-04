using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins.MsBuild
{
    [Export(typeof(IPlugin))]
    public class MsBuild : BasePlugin
    {
        private const string Program = "msbuild.exe";

        public override string InstallationInstructions
        {
            get { return "Something's really wrong if you're missing msbuild..."; }
        }

        public override string ShortHelp
        {
            get { return "Builds Visual Studio projects and solutions."; }
        }

        public override PluginConfigurations SampleSettings
        {
            get
            {
                return new PluginConfigurations() {
                    {
                        "Orchard",
                        new PluginConfiguration() {
                            { "Project", @"orchard\src\orchard.sln" }
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
                Shell.Info(string.Format("Building {0}...", c));
                var code = Shell.Run("msbuild.exe", Configuration[c]["Project"], ProcessOutput.Error, Config.Current.Path);
                if (code == 0)
                    Shell.Success("Done!");
                else
                    Shell.Error(string.Format("{0} build failed."));
            });
        }

        public override bool IsAvailable()
        {
            return Shell.IsAvailable(Program);
        }
    }
}
