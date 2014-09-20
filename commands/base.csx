public abstract class BaseCommand
{
  public string Name { get { return this.GetType().Name.ToLower(); } }
  public abstract string ShortHelp { get; }
  public abstract string LongHelp { get; }
  public abstract string[] SubCommands { get; }

  public virtual bool Run(string[] args)
  {
    if (SubCommands == null || SubCommands.Length == 0)
      return true;

    if (args.Length == 0)
    {
      Con.Error("Subcommand missing.");
      Con.Info("Run \"no3 help " + Name + "\" for help.");
      return false;
    }

    if (SubCommands.Contains(args[0]))
      return true;

    Con.Error("Invalid subcommand \"" + args[0] + "\".");
    Con.Info("Run \"no3 help " + Name + "\" for help.");
    return false;
  }
}
