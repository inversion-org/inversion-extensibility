using System.Collections.Generic;

namespace Inversion.Process.Behaviour
{

    public class SetParametersBehaviour : PrototypedBehaviour
    {
        public SetParametersBehaviour(string respondsTo) : base(respondsTo) { }
        public SetParametersBehaviour(string respondsTo, IConfiguration config) : base(respondsTo, config.Elements) { }
        public SetParametersBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) { }

        public override void Action(IEvent ev, IProcessContext context)
        {
            IDictionary<string, string> mappings = this.Configuration.GetMap("context", "set");
            foreach (KeyValuePair<string, string> entry in mappings)
            {
                context.Params[entry.Key] = entry.Value;
            }
        }
    }
}