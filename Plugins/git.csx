public class Git : BasePlugin
{
  private const string Program = "git.exe";

  public override string ShortHelp {
    get { return "Manages Git repositories."; }
  }

  public override string LongHelp {
    get { return @"Available subcommands:
 - clone:  clones the repository locally.
 - update: updates the local repository."; }
  }

  public override string[] SubCommands {
    get { return new string[] { "clone", "update" }; }
  }

  public override string InstallationInstructions
  {
    get { return "Please install git and make sure it's in your PATH."; }
  }

  public override PluginSettings DefaultSettings
  {
    get
    {
      return new PluginSettings() {
        {
          "Orchard",
          new Dictionary<string, string>() {
            { "Repo", "https://git01.codeplex.com/orchard" },
            { "Branch", "master" },
            { "Path", "orchard" }
          }
        }
      };
    }
  }

  public override bool IsAvailable()
  {
    return Con.IsAvailable(Program);
  }

  public override void Run(string subCommand, string configName, string[] args)
  {
    List<string> configs;
    if (configName == "all" || String.IsNullOrWhiteSpace(configName))
      configs = new List<string>(Configuration.Keys.ToArray());
    else
      configs = new List<string>() { configName };
    configs.ForEach(c => {
      switch(subCommand)
      {
        case "clone":
            Con.Run("git.exe", "clone " + Configuration[c]["Repo"] + " " + Configuration[c]["Path"], false, Config.Current.Path);
            Con.Run("git.exe", "checkout " + Configuration[c]["Branch"], true, Config.Current.Path);
          break;
        case "update":
          break;
      }
    });


    // if (base.Run(args))
    // {
    //   switch (args[0]) {
    //     case "clone":
    //       Con.Run("git.exe", "clone " + Config.Current.Settings["Orchard"]["Repo"]);
    //       Con.Run("git.exe", "checkout " + Config.Current.Settings["Orchard"]["Branch"]);
    //       break;
    //     case "update":
    //       break;
    //   }
    // }
  }
}

Plugins.Register(new Git());
