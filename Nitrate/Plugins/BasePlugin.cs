using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nitrate.Plugins
{
    public abstract class BasePlugin<T> : IPlugin
    {
        public string Name
        {
            get { return this.GetType().Name; }
        }

        public virtual IDictionary<string, SubCommand> SubCommands
        {
            get { return null; }
        }

        public virtual List<Argument> Arguments 
        {
            get { return null; }
        }

        public virtual string InstallationInstructions { get { return string.Empty; } }

        public abstract string Description { get; }

        public abstract void Execute(string configName, T config, string subCommand, Dictionary<string, string> args);

        protected abstract Dictionary<string, T> SampleConfiguration();

        public abstract bool IsAvailable();

        public void Execute(string configName, JObject config, string subCommand, Dictionary<string, string> args)
        {
            Execute(configName, config.ToObject<T>(), subCommand, args);
        }

        public Dictionary<string, JObject> GetSampleConfiguration()
        {
            var r = new Dictionary<string, JObject>();
            foreach (var pair in SampleConfiguration())
            {
                r.Add(pair.Key, JObject.FromObject(pair.Value));
            }

            return r;
        }
    }
}
