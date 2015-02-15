using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins.MsBuild
{
    public class MsBuildConfig
    {
        public string Project { get; set; }
    }

    [Export(typeof(IPlugin))]
    public class MsBuild : BasePlugin<MsBuildConfig>
    {
        private const string Program = "msbuild.exe";

        public override string InstallationInstructions
        {
            get { return "Something's really wrong if you're missing msbuild..."; }
        }

        public override string Description
        {
            get { return "Builds Visual Studio projects and solutions."; }
        }

        public override bool IsAvailable()
        {
            return Shell.IsAvailable(Program);
        }

        public override void Execute(string configName, MsBuildConfig config, string subCommand, Dictionary<string, string> args)
        {
            Shell.Info(string.Format("Building {0}...", configName));
            var code = Shell.Run("msbuild.exe", config.Project, ProcessOutput.Window, Config.Current.Path);
            if (code == 0)
                Shell.Success("Done!");
            else
                Shell.Error(string.Format("{0} build failed."));
        }

        protected override Dictionary<string, MsBuildConfig> SampleConfiguration()
        {
            return new Dictionary<string, MsBuildConfig>()
            {
                {
                    "Orchard", new MsBuildConfig
                    {
                        Project = @"orchard\src\orchard.sln"
                    }
                }
            };
        }
    }
}
