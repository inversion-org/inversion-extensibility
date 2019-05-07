using System;
using System.Collections.Generic;
using System.Linq;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class SynchronisedBlockBehaviour : PrototypedBehaviour
    {
        private readonly IList<IProcessBehaviour> _block;
        private readonly object _syncObject;

        public SynchronisedBlockBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config,
            IList<IProcessBehaviour> block, object syncObject) : base(respondsTo, config)
        {
            _block = block;
            _syncObject = syncObject;
        }

        public override bool Condition(IEvent ev, IProcessContext context)
        {
            return base.Condition(ev, context) &&
                   (
                       !this.Configuration.Has("config", "flag") ||
                       (this.Configuration.Has("config", "flag") &&
                        !context.IsFlagged(this.Configuration.GetNameWithAssert("config", "flag")))
                       );
        }

        public override void Action(IEvent ev, IProcessContext context)
        {
            if (this.Configuration.Has("config", "flag"))
            {
                string flagName = this.Configuration.GetNameWithAssert("config", "flag");

                // set flag to show we have dispatched
                context.Flags.Add(flagName);
            }

            lock (_syncObject)
            {
                foreach (IProcessBehaviour behaviour in _block.Where(behaviour => behaviour.Condition(ev, context)))
                {
                    ProcessContext.PreAction?.Invoke(behaviour, new ActionEventArgs(context: context, ev: ev));
                    behaviour.Action(ev, context);
                    ProcessContext.PostAction?.Invoke(behaviour, new ActionEventArgs(context: context, ev: ev));
                }
            }
        }
    }
}
