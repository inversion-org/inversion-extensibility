using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class CopyControlStateBehaviour : PrototypedBehaviour
    {
        public CopyControlStateBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            string inputKey = this.Configuration.GetNameWithAssert("config", "input-key");
            string outputKey = this.Configuration.GetNameWithAssert("config", "output-key");

            IData input = context.ControlState.GetWithAssert<IData>(inputKey);

            context.ControlState[outputKey] = input;
        }
    }
}