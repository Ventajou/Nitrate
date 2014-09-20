public class Clean : BaseCommand
{
  public override string ShortHelp {
    get { return "Cleans the environment"; }
  }

  public override string LongHelp {
    get { return @"Available subcommands:
 - all:      removes Orchard and everything.
 - mappings: removes the mappings.bin file."; }
  }

  public override string[] SubCommands {
    get { return new string[] { "all", "mappings" }; }
  }

  public override bool Run(string[] args)
  {
    if (base.Run(args))
    {

    }

    return true;
  }
}
