using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class FireEventFromControlStateBehaviour : PrototypedBehaviour
    {
        public FireEventFromControlStateBehaviour(string respondsTo) : base(respondsTo) {}
        public FireEventFromControlStateBehaviour(string respondsTo, IPrototype prototype) : base(respondsTo, prototype) {}
        public FireEventFromControlStateBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            string inputKey = this.Configuration.GetNameWithAssert("config", "input-key");

            IEvent inputEvent = context.ControlState.GetWithAssert<IEvent>(inputKey);

            context.Fire(inputEvent);
        }
    }
}