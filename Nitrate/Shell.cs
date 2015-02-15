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
        All,
        Window
    }

    public class Shell
    {
        public static void Write(string message, bool newLine = true, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.Write(message);
            if (newLine) Console.WriteLine();
            Console.ResetColor();
        }

        public static void Error(string message, bool newLine = true)
        {
            Write(message, newLine, ConsoleColor.DarkRed);
        }

        public static void Success(string message, bool newLine = true)
        {
            Write(message, newLine, ConsoleColor.DarkGreen);
        }

        public static void Info(string message, bool newLine = true)
        {
            Write(message, newLine, ConsoleColor.DarkCyan);
        }

        public static void Warn(string message, bool newLine = true)
        {
            Write(message, newLine, ConsoleColor.Yellow);
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
                CreateNoWindow = output != ProcessOutput.Window,
                UseShellExecute = output == ProcessOutput.Window,
                RedirectStandardOutput = (output == ProcessOutput.Standard || output == ProcessOutput.All),
                RedirectStandardError = (output == ProcessOutput.Error || output == ProcessOutput.All),
                FileName = app,
                Arguments = command
            };
            if (!String.IsNullOrWhiteSpace(workingDirectory)) p.StartInfo.WorkingDirectory = workingDirectory;

            p.Start();

            if (output == ProcessOutput.Standard || output == ProcessOutput.All)
            {
                p.OutputDataReceived += (sender, args) => Shell.Info(args.Data, false);
                p.BeginOutputReadLine();
            }

            if (output == ProcessOutput.Error || output == ProcessOutput.All)
            {
                p.ErrorDataReceived += (sender, args) => Shell.Error(args.Data, false);
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
