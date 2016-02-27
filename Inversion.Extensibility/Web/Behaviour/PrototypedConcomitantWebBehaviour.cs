using System.Collections.Generic;
using System.Linq;
using Inversion.Process;
using Inversion.Process.Behaviour;

namespace Inversion.Web.Behaviour
{
    public abstract class PrototypedConcomitantWebBehaviour : PrototypedWebBehaviour
    {
        private readonly IList<IProcessBehaviour> _success = new List<IProcessBehaviour>();
        private readonly IList<IProcessBehaviour> _failure = new List<IProcessBehaviour>();

        protected PrototypedConcomitantWebBehaviour(string respondsTo) : base(respondsTo) {}
        protected PrototypedConcomitantWebBehaviour(string respondsTo, IPrototype prototype,
            IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure) : base(respondsTo, prototype)
        {
            _success = new List<IProcessBehaviour>(success);
            _failure = new List<IProcessBehaviour>(failure);
        }

        protected PrototypedConcomitantWebBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config,
            IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure) : base(respondsTo, config)
        {
            _success = new List<IProcessBehaviour>(success);
            _failure = new List<IProcessBehaviour>(failure);
        }

        protected void Success(IEvent ev, IWebContext context)
        {
            this.Chain(ev, context, _success);
        }

        protected void Failure(IEvent ev, IWebContext context)
        {
            this.Chain(ev, context, _failure);
        }

        protected void Chain(IEvent ev, IWebContext context, IList<IProcessBehaviour> chain)
        {
            foreach (IProcessBehaviour behaviour in chain.Where(behaviour => behaviour.Condition(ev, context)))
            {
                behaviour.Action(ev, context);
            }
        }
    }
}