using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inversion.Extensibility.Extensions;
using log4net;

namespace Inversion.Process.Behaviour
{
    public abstract class PrototypedConcomitantBehaviour : PrototypedBehaviour
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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
                ProcessContext.PreAction?.Invoke(behaviour, new ActionEventArgs(context: context, ev: ev));
                behaviour.Action(ev, context);
                ProcessContext.PostAction?.Invoke(behaviour, new ActionEventArgs(context: context, ev: ev));
            }
        }
    }
}
