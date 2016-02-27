using System.Collections.Generic;
using System.Linq;

namespace Inversion.Process.Behaviour.Eval
{
    public abstract class PrototypedConcomitantEvaluatingBehaviour : PrototypedEvaluatingBehaviour
    {
        private readonly IList<IProcessBehaviour> _success = new List<IProcessBehaviour>();
        private readonly IList<IProcessBehaviour> _failure = new List<IProcessBehaviour>();

        protected PrototypedConcomitantEvaluatingBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config,
            IEnumerable<IConfigurationElement> evals, IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure) : base(respondsTo, config, evals)
        {
            _success = new List<IProcessBehaviour>(success);
            _failure = new List<IProcessBehaviour>(failure);
        }

        protected void Success(IEvent ev, IProcessContext context)
        {
            this.Chain(ev, context, _success);
        }

        protected void Failure(IEvent ev, IProcessContext context)
        {
            this.Chain(ev, context, _failure);
        }

        protected void Chain(IEvent ev, IProcessContext context, IList<IProcessBehaviour> chain)
        {
            foreach (IProcessBehaviour behaviour in chain.Where(behaviour => behaviour.Condition(ev, context)))
            {
                behaviour.Action(ev, context);
            }
        }
    }
}