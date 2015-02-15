using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins
{
    public class Argument
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Regex { get; set; }
    }

    public class SubCommand
    {
        public string Description { get; set; }
        public string Example { get; set; }
        public List<Argument> Arguments { get; set;}
    }
}
