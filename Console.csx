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

  public static string Run(string app, string command, bool redirectOutput = true, string workingDirectory = "")
  {
  	var p = new Process();
  	p.StartInfo = new ProcessStartInfo() {
  		CreateNoWindow = true,
  		UseShellExecute = false,
  		RedirectStandardOutput = redirectOutput,
  		FileName = app,
  		Arguments = command
  	};
    if (!String.IsNullOrWhiteSpace(workingDirectory)) p.StartInfo.WorkingDirectory = workingDirectory;
  	p.Start();
    string output = string.Empty;
  	if (redirectOutput) output = p.StandardOutput.ReadToEnd();
  	p.WaitForExit();
  	p.Close();
  	return output;
  }

  public static bool IsAvailable(String fileName)
  {
      string path = Environment.GetEnvironmentVariable("path");
      string[] folders = path.Split(';');

      foreach (var folder in folders)
      {
          if (File.Exists(Path.Combine(folder, fileName)))
          {
              return true;
          }
      }

      return false;
  }
}
