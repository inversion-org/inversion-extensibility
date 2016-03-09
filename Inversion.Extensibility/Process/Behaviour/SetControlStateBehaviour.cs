using System.Collections.Generic;

namespace Inversion.Process.Behaviour
{
    public class SetControlStateBehaviour : PrototypedBehaviour
    {
        public SetControlStateBehaviour(string respondsTo) : base(respondsTo) {}
        public SetControlStateBehaviour(string respondsTo, IPrototype prototype) : base(respondsTo, prototype) {}
        public SetControlStateBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            foreach (IConfigurationElement element in this.Configuration.GetElements("control-state", "set"))
            {
                context.ControlState[element.Name] = new NamedTextData(element.Name, element.Value);
            }
        }
    }
}