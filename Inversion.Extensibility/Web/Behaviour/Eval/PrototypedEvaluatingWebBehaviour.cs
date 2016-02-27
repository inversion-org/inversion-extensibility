using System.Collections.Generic;
using Inversion.Process;
using Inversion.Process.Behaviour.Eval;

namespace Inversion.Web.Behaviour.Eval
{
    public abstract class PrototypedEvaluatingWebBehaviour : PrototypedEvaluatingBehaviour, IWebBehaviour
    {

        public PrototypedEvaluatingWebBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config, IEnumerable<IConfigurationElement> evals) : base(respondsTo, config, evals) {}

        public override bool Condition(IEvent ev)
        {
            return this.Condition(ev, (IWebContext) ev.Context);
        }

        public virtual bool Condition(IEvent ev, IWebContext context)
        {
            return base.Condition(ev, context);
        }

        public override void Action(IEvent ev)
        {
            this.Action(ev, ev.Context);
        }

        public override void Action(IEvent ev, IProcessContext context)
        {
            this.Action(ev, (IWebContext)context);
        }

        public abstract void Action(IEvent ev, IWebContext context);
    }
}