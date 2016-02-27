using System;
using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public abstract class LoopBehaviour : PrototypedBehaviour
    {
        private readonly IList<IProcessBehaviour> _loop;

        protected LoopBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config, IList<IProcessBehaviour> loop) : base(respondsTo, config)
        {
            _loop = loop;
        }

        protected virtual IProcessContext ConfigureLoop(IProcessContext context)
        {
            IProcessContext loopContext = new LoopProcessContext(context, context.Services, context.Resources);

            loopContext.Register(_loop);

            foreach (KeyValuePair<string, string> kvp in context.Params)
            {
                loopContext.Params[kvp.Key] = kvp.Value;
            }

            foreach (KeyValuePair<string, object> kvp in context.ControlState)
            {
                loopContext.ControlState[kvp.Key] = kvp.Value;
            }

            return loopContext;
        }

        protected abstract IEnumerable<bool> LoopCondition(IEvent ev, IProcessContext loopContext);

        public override void Action(IEvent ev, IProcessContext context)
        {
            int yieldTimeMs = Convert.ToInt32(this.Configuration.GetNameWithAssert("config", "yieldtimems"));
            string message = this.Configuration.GetOverride("loop-message", "work");

            // create a context and copy params and current control state into it
            IProcessContext loopContext = this.ConfigureLoop(context);

            bool firstTime = true;

            using (IEnumerator<bool> enumerator = this.LoopCondition(ev, loopContext).GetEnumerator())
            {
                do
                {
                    enumerator.MoveNext();

                    if (!enumerator.Current)
                    {
                        continue;
                    }

                    // only yield if we've been through the cycle before
                    // don't want to arbitrarily yield after firing the event
                    if (!firstTime)
                    {
                        System.Threading.Thread.Sleep(yieldTimeMs);
                    }
                    firstTime = false;

                    loopContext.Fire(new Event(loopContext, message));
                    
                } while (enumerator.Current);
            }
        }
    }
}