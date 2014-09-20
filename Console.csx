using System.Diagnostics;
using System.IO;

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

  public static string Run(string app, string command)
  {
  	var p = new Process();
  	p.StartInfo = new ProcessStartInfo() {
  		CreateNoWindow = true,
  		UseShellExecute = false,
  		RedirectStandardOutput = true,
  		FileName = app,
  		Arguments = command
  	};
  	p.Start();
  	var output = p.StandardOutput.ReadToEnd();
  	p.WaitForExit();
  	p.Close();
  	return output;
  }
}
