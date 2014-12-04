using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins
{
    public interface IPlugin
    {
        /// <summary>
        /// A friendly name for the component.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Installation instructions if a necessary external tool is missing.
        /// </summary>
        string InstallationInstructions { get; }
        
        /// <summary>
        /// Short help line to be shown on the help index.
        /// </summary>
        string ShortHelp { get; }
        
        /// <summary>
        /// Long help text to be shown on the plugin specific help page.
        /// </summary>
        string LongHelp { get; }
        
        /// <summary>
        /// Name of the subcommands supported by this plugin
        /// </summary>
        string[] SubCommands { get; }

        /// <summary>
        /// Sample settings used when initializing the project
        /// </summary>
        PluginConfigurations SampleSettings { get; }

        /// <summary>
        /// Runs the plugin with provided arguments
        /// </summary>
        /// <param name="args"></param>
        void Run(string subCommand, string configName, string[] args);
        
        /// <summary>
        /// Checks if the plugin is available.
        /// </summary>
        /// <returns></returns>
        bool IsAvailable();

        /// <summary>
        /// Configures the plugin 
        /// </summary>
        /// <param name="settings"></param>
        void Configure(PluginConfigurations settings);

        PluginConfigurations Configuration { get; }
    }
}
