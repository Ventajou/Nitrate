#load "console.csx"
#load "config.csx"
#load "commands.csx"

using System.Linq;

Con.Lf();
Con.Info("Nitrate - Grow your Orchard faster");
Con.Lf();

var config = Config.Load();

var command = (Env.ScriptArgs.Count() > 0) ? Env.ScriptArgs[0] : null;

if (!String.IsNullOrWhiteSpace(command)) command = command.ToLower();

if (config == null)
{
  if (command == "init")
  {
    Con.Info("Initializing project...");
    Config.Init().Save();
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
    Commands.Run(command, Env.ScriptArgs.Skip(1).ToArray());
}

Con.Lf();

/*

  Modules:

  - orchard
  - IIS
  - SQL Server

*/
