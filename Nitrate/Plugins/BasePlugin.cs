using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        public string Name
        {
            get { return this.GetType().Name; }
        }

        public virtual string[] SubCommands
        {
            get { return new string[0]; }
        }

        public PluginConfigurations Configuration { get; private set; }

        public void Configure(PluginConfigurations settings)
        {
            Configuration = settings;
        }

        public virtual string InstallationInstructions { get { return string.Empty; } }

        public abstract string ShortHelp { get; }

        public virtual string LongHelp { get { return ShortHelp; } }

        public abstract PluginConfigurations SampleSettings { get; }

        public abstract void Run(string subCommand, string configName, string[] args);

        public abstract bool IsAvailable();
    }
}
