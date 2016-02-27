using System.Collections.Generic;
using System.Linq;

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

        public override void Action(IEvent ev, IProcessContext context)
        {
            foreach (IProcessBehaviour behaviour in _block.Where(behaviour => behaviour.Condition(ev, context)))
            {
                behaviour.Action(ev, context);
            }
        }
    }
}