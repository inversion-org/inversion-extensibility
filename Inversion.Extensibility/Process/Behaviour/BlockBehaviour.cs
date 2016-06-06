using System.Collections.Generic;
using System.Linq;

using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class BlockBehaviour : PrototypedBehaviour
    {
        private readonly IList<IProcessBehaviour> _block;

        public BlockBehaviour(string respondsTo, IList<IProcessBehaviour> block) : base(respondsTo)
        {
            _block = block;
        }

        public BlockBehaviour(string respondsTo, IPrototype prototype, IList<IProcessBehaviour> block) : base(respondsTo, prototype)
        {
            _block = block;
        }

        public BlockBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config, IList<IProcessBehaviour> block) : base(respondsTo, config)
        {
            _block = block;
        }

        public override bool Condition(IEvent ev, IProcessContext context)
        {
            return base.Condition(ev, context) ||
                (this.Configuration.Has("config", "flag") && !context.IsFlagged(this.Configuration.GetNameWithAssert("config", "flag")));
        }

        public override void Action(IEvent ev, IProcessContext context)
        {
            if(this.Configuration.Has("config", "flag"))
            {
                string flagName = this.Configuration.GetNameWithAssert("config", "flag");

                // set flag to show we have dispatched
                context.Flags.Add(flagName);
            }

            foreach (IProcessBehaviour behaviour in _block.Where(behaviour => behaviour.Condition(ev, context)))
            {
                behaviour.Action(ev, context);
            }
        }
    }
}