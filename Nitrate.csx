#load "console.csx"
#load "config.csx"

Con.Lf();
Con.Info("Nitrate - Grow your Orchard faster");
Con.Lf();

var config = Config.Load();

var command = (Env.ScriptArgs.Count() > 0) ? Env.ScriptArgs[0] : null;
if (!String.IsNullOrWhiteSpace(command)) command = command.ToLower();

if (config == null)
{
  if (command != "init")
  {
    Con.Error("Couldn't find nitrate.json.");
    Con.Info("Run no3 init to initialize a project in the current directory.");
    Con.Lf();
  }
  else
  {
    Con.Info("Initializing project...");
    Config.Init().Save();
    Con.Success("Done!");
  }
}
else
{

}
/*

  Modules:

  - orchard
  - IIS
  - SQL Server

*/
