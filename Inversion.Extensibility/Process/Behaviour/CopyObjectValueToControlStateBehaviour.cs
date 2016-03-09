using System.Collections.Generic;
using Inversion.Extensibility.Extensions;
using Inversion.Process.Behaviour.Eval;

namespace Inversion.Process.Behaviour
{
    public class CopyObjectValueToControlStateBehaviour : PrototypedEvaluatingBehaviour
    {
        public CopyObjectValueToControlStateBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config, IEnumerable<IConfigurationElement> evals) : base(respondsTo, config, evals) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            IDictionary<string, string> data = this.Evaluate(context);
            string value = data.GetEvalDataWithAssert("value");

            string outputKey = this.Configuration.GetNameWithAssert("config", "output-key");

            context.ControlState[outputKey] = new TextData(value);
        }
    }
}