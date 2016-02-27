using System;
using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour.Eval
{
    public abstract class PrototypedEvaluatingBehaviour : PrototypedBehaviour
    {
        private readonly IPrototype _evalPrototype;

        public IConfiguration Eval
        {
            get { return _evalPrototype; }
        }

        protected PrototypedEvaluatingBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config, IEnumerable<IConfigurationElement> evals) : base(respondsTo, config)
        {
            _evalPrototype = new Prototype(evals);
        }

        public IDictionary<string, string> Evaluate(IProcessContext context)
        {
            IDictionary<string, string> data = new Dictionary<string, string>();

            foreach (IConfigurationElement element in this.Eval.Elements)
            {
                string targetKey = element.Frame;
                string res = String.Empty;
                if (element.Slot == "control-state")
                {
                    res = context.ControlState.GetEffectiveStringResult(element.Name);
                }
                else if (element.Slot == "context")
                {
                    res = context.ParamOrDefault(element.Name, String.Empty);
                }
                else if (element.Slot == "object-cache")
                {
                    res = (string) context.ObjectCache.Get(element.Name);
                }
                if (res != null)
                {
                    data[targetKey] = res;
                }
            }

            return data;
        }
    }
}