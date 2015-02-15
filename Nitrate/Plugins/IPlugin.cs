using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        string Description { get; }
        
        /// <summary>
        /// Name and description of the subcommands supported by this plugin, if any
        /// </summary>
        IDictionary<string, SubCommand> SubCommands { get; }

        /// <summary>
        /// Arguments supported by plugins with no subcommands
        /// </summary>
        List<Argument> Arguments { get; }
        
        /// <summary>
        /// Runs the plugin with provided arguments
        /// </summary>
        /// <param name="args"></param>
        void Execute(string configName, JObject config, string subCommand, Dictionary<string, string> args);

        /// <summary>
        /// Returns a sample configuration for the plugin
        /// </summary>
        /// <returns></returns>
        Dictionary<string, JObject> GetSampleConfiguration();

        /// <summary>
        /// Checks if the plugin is available.
        /// </summary>
        /// <returns></returns>
        bool IsAvailable();
    }
}
