using System;
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

            string overrideEventName = this.Configuration.Has("config", "override-event-name")
                ? this.Configuration.GetNameWithAssert("config", "override-event-name")
                : String.Empty;

            IEvent inputEvent = context.ControlState.GetWithAssert<IEvent>(inputKey);

            if (!String.IsNullOrEmpty(overrideEventName))
            {
                context.Fire(overrideEventName, inputEvent.Params);
            }
            else
            {
                context.Fire(inputEvent);
            }
        }
    }
}