using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Nitrate
{
    public enum ProcessOutput
    {
        None,
        Error,
        Standard,
        All
    }

    public class Shell
    {
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

        public static int Run(string app, string command, ProcessOutput output = ProcessOutput.None, string workingDirectory = "")
        {
            var p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = (output == ProcessOutput.Standard || output == ProcessOutput.All),
                RedirectStandardError = (output == ProcessOutput.Error || output == ProcessOutput.All),
                FileName = app,
                Arguments = command
            };
            if (!String.IsNullOrWhiteSpace(workingDirectory)) p.StartInfo.WorkingDirectory = workingDirectory;

            p.Start();

            if (output == ProcessOutput.Standard || output == ProcessOutput.All)
            {
                p.OutputDataReceived += (sender, args) => Shell.Info(args.Data);
                p.BeginOutputReadLine();
            }

            if (output == ProcessOutput.Error || output == ProcessOutput.All)
            {
                p.ErrorDataReceived += (sender, args) => Shell.Error(args.Data);
                p.BeginErrorReadLine();
            }
            p.WaitForExit();
            var exitCode = p.ExitCode;
            p.Close();
            return exitCode;
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
}
