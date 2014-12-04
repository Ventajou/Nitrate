using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Help: http://www.codeproject.com/Articles/23823/Create-a-SQL-Database-Programmatically

namespace Nitrate.Plugins.SqlServer
{
    [Export(typeof(IPlugin))]
    public class SqlServer : BasePlugin
    {
        public override string ShortHelp
        {
            get { return "Manages local SQL Server databases"; }
        }

        public override string[] SubCommands
        {
            get { return new string[] { "create", "backup", "restore", "drop" }; }
        }

        public override PluginConfigurations SampleSettings
        {
            get
            {
                return new PluginConfigurations() {
                    {
                      "Orchard",
                      new PluginConfiguration() {
                      }
                    }
                };
            }
        }

        public override void Run(string subCommand, string configName, string[] args)
        {
            throw new NotImplementedException();
        }

        public override bool IsAvailable()
        {
            return true;
        }
    }
}
