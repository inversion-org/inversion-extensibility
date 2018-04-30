using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Inversion.Extensibility.Extensions;
using log4net;

namespace Inversion.Process.Behaviour
{
    public class BlockBehaviour : PrototypedBehaviour
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IList<IProcessBehaviour> Block;

        public BlockBehaviour(string respondsTo, IList<IProcessBehaviour> block) : base(respondsTo)
        {
            this.Block = block;
        }

        public BlockBehaviour(string respondsTo, IPrototype prototype, IList<IProcessBehaviour> block) : base(respondsTo, prototype)
        {
            this.Block = block;
        }

        public BlockBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config, IList<IProcessBehaviour> block) : base(respondsTo, config)
        {
            this.Block = block;
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
            bool logActions = this.Configuration.Has("config", "log")
                ? Convert.ToBoolean(this.Configuration.GetNameWithAssert("config", "log"))
                : true;

            if(this.Configuration.Has("config", "flag"))
            {
                string flagName = this.Configuration.GetNameWithAssert("config", "flag");

                // set flag to show we have dispatched
                context.Flags.Add(flagName);
            }

            foreach (IProcessBehaviour behaviour in this.Block.Where(behaviour => behaviour.Condition(ev, context)))
            {
                if (logActions)
                {
                    _log.DebugFormat("block action: {0}", behaviour.GetType().FullName);
                }
                behaviour.Action(ev, context);
            }
        }
    }
}