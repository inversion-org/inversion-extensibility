using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class SetContextItemsBehaviour : PrototypedBehaviour
    {
        public SetContextItemsBehaviour(string respondsTo) : base(respondsTo) {}
        public SetContextItemsBehaviour(string respondsTo, IPrototype prototype) : base(respondsTo, prototype) {}
        public SetContextItemsBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            foreach (IConfigurationElement element in this.Configuration.GetElements("control-state", "set"))
            {
                context.ControlState[element.Name] = new NamedTextData(element.Name, element.Value);
            }

            foreach (IConfigurationElement element in this.Configuration.GetElements("control-state", "remove"))
            {
                context.ControlState.Remove(element.Name);
            }

            foreach (KeyValuePair<string, string> entry in this.Configuration.GetMap("context", "set"))
            {
                context.Params[entry.Key] = entry.Value;
            }

            foreach (KeyValuePair<string, string> entry in this.Configuration.GetMap("context", "set-eval"))
            {
                context.Params[entry.Key] = context.ControlState.GetEffectiveStringResult(entry.Value);
            }

            foreach (IConfigurationElement element in this.Configuration.GetElements("context", "remove"))
            {
                context.Params.Remove(element.Name);
            }

            foreach (KeyValuePair<string, string> entry in this.Configuration.GetMap("event", "set"))
            {
                ev.Params[entry.Key] = entry.Value;
            }

            foreach (IConfigurationElement element in this.Configuration.GetElements("event", "remove"))
            {
                ev.Params.Remove(element.Name);
            }

            foreach (IConfigurationElement element in this.Configuration.GetElements("flag", "set"))
            {
                context.Flags.Add(element.Name);
            }

            foreach (IConfigurationElement element in this.Configuration.GetElements("flag", "remove"))
            {
                context.Flags.Remove(element.Name);
            }
        }
    }
}