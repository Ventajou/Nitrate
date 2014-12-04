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
    class Program
    {
        static void Main(string[] args)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new DirectoryCatalog(Path.Combine(Path.GetDirectoryName(executingAssembly.Location), "Plugins"), "*.dll"));
            catalog.Catalogs.Add(new AssemblyCatalog(executingAssembly));

            var container = new CompositionContainer(catalog);
            var pluginManager = new PluginManager();
            container.SatisfyImportsOnce(AttributedModelServices.CreatePart(pluginManager));
            
            Shell.Lf();
            Shell.Info("Nitrate - Grow your Orchard faster");
            Shell.Lf();

            var config = Config.Load(Environment.CurrentDirectory, pluginManager);

            var command = (args.Length > 0) ? args[0] : null;

            if (!String.IsNullOrWhiteSpace(command)) command = command.ToLower();

            if (config == null)
            {
                if (command == "init")
                {
                    Shell.Info("Initializing project...");
                    Config.Init(Environment.CurrentDirectory, pluginManager).Save();
                    Shell.Success("Done!");
                }
                else
                {
                    Shell.Error("Couldn't find nitrate.json.");
                    Shell.Info("Run no3 init to initialize a project in the current directory.");
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
                    pluginManager.Run(command, args.Skip(1).ToArray());
                }
            }

            Shell.Lf();
        }
    }
}
