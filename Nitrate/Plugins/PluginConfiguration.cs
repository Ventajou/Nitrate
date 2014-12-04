using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins
{
    public class PluginConfiguration : Dictionary<string, string>
    {
        public PluginConfiguration() : base(StringComparer.OrdinalIgnoreCase)
        { }

        public PluginConfiguration(IDictionary<string, string> initializer)
            : base(StringComparer.OrdinalIgnoreCase)
        { 
            foreach(var item in initializer)
            {
                Add(item.Key, item.Value);
            }
        }
    }
}
