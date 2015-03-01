using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nitrate
{
    public class Program
    {
        /// <summary>
        /// Finds and loads all of the plugins from the .\Plugins folder
        /// </summary>
        private static PluginManager LoadPlugins()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var catalog = new SafeDirectoryCatalog(Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "Plugins"));
            // Uncomment if plugins ever get bundled
            // catalog.Catalogs.Add(new AssemblyCatalog(executingAssembly));

            var container = new CompositionContainer(catalog);
            var pluginManager = new PluginManager();
            container.SatisfyImportsOnce(AttributedModelServices.CreatePart(pluginManager));

            return pluginManager;
        }

        public static PluginManager PluginManager { get; private set; }

        static void Main(string[] args)
        {
            PluginManager = LoadPlugins();
            
            Shell.Lf();
            Shell.Info("Nitrate - Grow your project faster");
            Shell.Lf();

            var config = Config.Load(Environment.CurrentDirectory);

            var command = (args.Length > 0) ? args[0] : null;

            if (!String.IsNullOrWhiteSpace(command)) command = command.ToLower();

            if (config == null)
            {
                if (command == "init")
                {
                    Shell.Write("Initializing project...");
                    Config.Init(Environment.CurrentDirectory, PluginManager.GetSampleConfiguration()).Save();
                    Shell.Success("Done!");
                }
                else
                {
                    Shell.Error("Couldn't find nitrate.json.");
                    Shell.Write("Run no3 init to initialize a project in the current directory.");
                    Shell.Lf();
                }
            }
            else
            {
                if (String.IsNullOrWhiteSpace(command)) command = "help";

                if (command == "init")
                    Shell.Error("Init command not available: you are already in a Nitrate project.");
                else
                {
                    PluginManager.Run(command, args.Skip(1).ToArray());
                }
            }

            Shell.Lf();
        }
    }
}
