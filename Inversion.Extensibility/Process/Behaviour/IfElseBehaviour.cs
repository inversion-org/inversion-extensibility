using System;
using System.Collections.Generic;
using System.Linq;

namespace Inversion.Process.Behaviour
{
    public class IfElseBehaviour : PrototypedConcomitantBehaviour
    {
        protected readonly IPrototype DecisionPrototype;

        public IfElseBehaviour(string respondsTo,
            IEnumerable<IConfigurationElement> config,
            IEnumerable<IConfigurationElement> criteria,
            IEnumerable<IProcessBehaviour> pass,
            IEnumerable<IProcessBehaviour> fail) : base(respondsTo, config, pass, fail)
        {
            this.DecisionPrototype = new Prototype(criteria);
        }

        public override void Action(IEvent ev, IProcessContext context)
        {
            if (this.DecisionPrototype.Criteria.All(criteria => criteria(this.DecisionPrototype, ev)))
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