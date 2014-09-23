#load "commands\base.csx"
#load "commands\clean.csx"
#load "commands\orchard.csx"

public class Commands
{
  private static List<BaseCommand> _commands = new List<BaseCommand>();

  public static void Register(BaseCommand command)
  {
    _commands.Add(command);
  }

  public static void Run(string commandName, string[] args)
  {
    if (commandName == "help")
    {
      if (args.Length == 0 || args.Length > 1)
      {
        Con.Info("Usage: no3 <command> <options>");
        Con.Lf();
        Con.Info("Available Commands:");
        _commands.ForEach(c => Con.Info(" - " + c.Name + ": " + c.ShortHelp));
        Con.Lf();
      }
      else
      {
        var command = _commands.FirstOrDefault(c => c.Name == args[0]);
        if (command == null)
          Con.Error(args[0] + " is not a command.");
        else
        {
          Con.Info("Usage: no3 " + args[0] + " <options>");
          Con.Lf();
          Con.Info(command.LongHelp);
        }
      }
    }
    else
    {
      var command = _commands.FirstOrDefault(c => c.Name == commandName);
      if (command == null)
        Con.Error(commandName + " is not a command.");
      else
      {
        command.Run(args);
      }
    }
  }
}

Commands.Register(new Clean());
Commands.Register(new Orchard());
