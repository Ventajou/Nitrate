public class Orchard : BaseCommand
{
  public override string ShortHelp {
    get { return "Performs Orchard related tasks"; }
  }

  public override string LongHelp {
    get { return @"Available subcommands:
 - clone:  clones the Orchard repository locally.
 - update: updates the local copy of Orchard."; }
  }

  public override string[] SubCommands {
    get { return new string[] { "clone", "update" }; }
  }

  public override bool Run(string[] args)
  {
    if (base.Run(args))
    {

    }

    return true;
  }
}
