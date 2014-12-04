using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins
{
    public class PluginConfigurations : Dictionary<string, PluginConfiguration>
    {
        public PluginConfigurations() : base(StringComparer.OrdinalIgnoreCase)
        { }

        public PluginConfigurations(IDictionary<string, PluginConfiguration> initializer)
            : base(StringComparer.OrdinalIgnoreCase)
        { 
            foreach(var item in initializer)
            {
                Add(item.Key, item.Value);
            }
        }
    }
}
