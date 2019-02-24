using System;
using System.Collections.Generic;
using System.Linq;

namespace Inversion.Process.Behaviour
{
    public class OrBehaviour : PrototypedConcomitantBehaviour
    {
        protected readonly List<IPrototype> DecisionPrototypes;

        public OrBehaviour(string respondsTo,
            IEnumerable<IConfigurationElement> config,
            List<IEnumerable<IConfigurationElement>> criteria,
            IEnumerable<IProcessBehaviour> pass,
            IEnumerable<IProcessBehaviour> fail) : base(respondsTo, config, pass, fail)
        {
            this.DecisionPrototypes = new List<IPrototype>(criteria.Select(c => new Prototype(c)));
        }

        public override void Action(IEvent ev, IProcessContext context)
        {
            if (this.DecisionPrototypes.Any(
                decisionPrototype => decisionPrototype.Criteria.All(
                    criteria => criteria(decisionPrototype, ev))))
            {
                this.Success(ev, context);
            }
            else
            {
                this.Failure(ev, context);
            }
        }
    }
}