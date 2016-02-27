using System.Collections.Generic;
using System.Linq;

namespace Inversion.Process.Behaviour
{
    public abstract class PrototypedConcomitantBehaviour : PrototypedBehaviour
    {
        private readonly IList<IProcessBehaviour> _success = new List<IProcessBehaviour>();
        private readonly IList<IProcessBehaviour> _failure = new List<IProcessBehaviour>();

        protected PrototypedConcomitantBehaviour(string respondsTo) : base(respondsTo) {}
        protected PrototypedConcomitantBehaviour(string respondsTo, IPrototype prototype,
            IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure) : base(respondsTo, prototype)
        {
            _success = new List<IProcessBehaviour>(success);
            _failure = new List<IProcessBehaviour>(failure);
        }

        protected PrototypedConcomitantBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config,
            IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure) : base(respondsTo, config)
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