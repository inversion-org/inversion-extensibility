using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class CopyObjectValueToControlStateBehaviour : PrototypedBehaviour
    {
        public CopyObjectValueToControlStateBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            IDictionary<string, string> data = this.Configuration.Evaluate(ev, context);
            string value = data.GetWithAssert("value");

            string outputKey = this.Configuration.GetNameWithAssert("config", "output-key");

            context.ControlState[outputKey] = new TextData(value);
        }
    }
}