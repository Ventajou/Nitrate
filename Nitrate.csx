#load "console.csx"
#load "config.csx"
#load "plugins.csx"

using System.Linq;

Con.Lf();
Con.Info("Nitrate - Grow your Orchard faster");
Con.Lf();

var config = Config.Load(Env.ScriptArgs[0]);

var command = (Env.ScriptArgs.Count() > 1) ? Env.ScriptArgs[1] : null;

if (!String.IsNullOrWhiteSpace(command)) command = command.ToLower();

if (config == null)
{
  if (command == "init")
  {
    Con.Info("Initializing project...");
    Config.Init(Env.ScriptArgs[0]).Save();
    Con.Success("Done!");
  }
  else
  {
    Con.Error("Couldn't find nitrate.json.");
    Con.Info("Run no3 init to initialize a project in the current directory.");
    Con.Lf();
  }
}
else
{
  if (String.IsNullOrWhiteSpace(command)) command = "help";

  if (command == "init")
    Con.Error("Init command not available: you are already in a Nitrate project.");
  else
  {
    Plugins.Run(command, Env.ScriptArgs.Skip(2).ToArray());
  }
}

Con.Lf();
