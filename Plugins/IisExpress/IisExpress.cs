using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins.IisExpress
{
    public class IisExpressConfig
    {
        public string ClrVersion { get; set; }
        public int Port { get; set; }
        public string Path { get; set; }
        public bool Systray { get; set; }
        public bool BrowseOnStart { get; set; }

        public IisExpressConfig()
        {
            Systray = true;
            Port = 8080;
        }
    }

    [Export(typeof(IPlugin))]
    public class IisExpress : BasePlugin<IisExpressConfig>
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetTopWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const string Program = @"C:\Program Files\IIS Express\iisexpress.exe";
        private static class Commands
        {
            public const string Start = "start";
            public const string Stop = "stop";
        }

        public override string InstallationInstructions
        {
            get { return "Please install IIS Express."; }
        }

        public override string Description
        {
            get { return "Manages IIS Express."; }
        }

        private IDictionary<string, SubCommand> _subCommands;
        public override IDictionary<string, SubCommand> SubCommands
        {
            get
            {
                if (_subCommands == null)
                {
                    _subCommands = new Dictionary<string, SubCommand>() { 
                        { Commands.Start, new SubCommand() { Description = "starts an IIS Express instance" } },
                        { Commands.Stop, new SubCommand() { Description = "stops the IIS Express instance" } }
                    };
                }
                return _subCommands;
            }
        }

        public override bool IsAvailable()
        {
            return Shell.IsAvailable(Program);
        }

        public override void Execute(string configName, IisExpressConfig config, string subCommand, Dictionary<string, string> args)
        {
            var pidFileName = Path.Combine(Config.Current.WorkfilesPath, "iisexpress." + configName + ".pid");
            
            switch (subCommand)
            {
                case Commands.Start:
                    Shell.Write("Starting IIS Express (" + configName + ")...");

                    StringBuilder arguments = new StringBuilder();
                    if (!string.IsNullOrEmpty(config.Path))
                        arguments.AppendFormat("/path:{0} ", Path.Combine(Config.Current.Path, config.Path));

                    if (!string.IsNullOrEmpty(config.ClrVersion))
                        arguments.AppendFormat("/clr:{0} ", config.ClrVersion);

                    arguments.AppendFormat("/systray:{0} /port:{1}", config.Systray, config.Port);

                    var process = Process.Start(new ProcessStartInfo()
                    {
                        FileName = Program,
                        Arguments = arguments.ToString(),
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    File.WriteAllText(pidFileName, process.Id.ToString());
                    if (config.BrowseOnStart)
                    {
                        Process.Start("http://localhost:" + config.Port);
                    }
                    Shell.Success("Done.");
                    break;

                case Commands.Stop:
                    Shell.Write("Stopping IIS Express (" + configName + ")...");

                    if (!File.Exists(pidFileName))
                    {
                        Shell.Error("The IIS Express process id could not be found.");
                        return;
                    }

                    int pid;
                    if (!int.TryParse(File.ReadAllText(pidFileName), out pid))
                    {
                        Shell.Error("The IIS Express process id could not be found.");
                        return;
                    }

                    try
                    {
                        for (IntPtr ptr = GetTopWindow(IntPtr.Zero); ptr != IntPtr.Zero; ptr = GetWindow(ptr, 2))
                        {
                            uint num;
                            GetWindowThreadProcessId(ptr, out num);
                            if (pid == num)
                            {
                                HandleRef hWnd = new HandleRef(null, ptr);
                                PostMessage(hWnd, 0x12, IntPtr.Zero, IntPtr.Zero);
                                Shell.Success("Stop message sent to the IIS Express process");
                                return;
                            }
                        }
                    }
                    catch (ArgumentException)
                    {
                    }
                    Shell.Error("The IIS Express process could not be found.");

                    break;
            }
        }

        protected override Dictionary<string, IisExpressConfig> SampleConfiguration()
        {
            return new Dictionary<string, IisExpressConfig>()
            {
                {
                    "Orchard", new IisExpressConfig
                    {
                        ClrVersion = "4.0",
                        Port = 8080,
                        Path = @"orchard\src\Orchard.Web",
                        Systray = true,
                        BrowseOnStart = true
                    }
                }
            };
        }
    }

}
