public class Con {
  public static void Error(string message)
  {
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.WriteLine(message);
    Console.ResetColor();
  }

  public static void Info(string message)
  {
    Console.WriteLine(message);
  }

  public static void Success(string message)
  {
    Console.ForegroundColor = ConsoleColor.DarkGreen;
    Console.WriteLine(message);
    Console.ResetColor();
  }

  public static void Lf()
  {
    Console.WriteLine();
  }
}
