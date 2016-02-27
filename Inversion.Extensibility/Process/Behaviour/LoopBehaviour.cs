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

        protected virtual LoopProcessContext ConfigureLoop(IProcessContext context)
        {
            // create a loop context which has the list of behaviours registered and a
            // copy of all parameters and control state objects.

            LoopProcessContext loopContext = new LoopProcessContext(context, context.Services, context.Resources);

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

        protected abstract IEnumerable<bool> LoopCondition(IEvent ev, LoopProcessContext loopContext);

        protected virtual void Finish(IEvent ev, LoopProcessContext loopContext) { }

        public override void Action(IEvent ev, IProcessContext context)
        {
            // get optional yield time to wait between iterations
            int yieldTimeMs = this.Configuration.Has("config", "yieldtimems")
                ? Convert.ToInt32(this.Configuration.GetNameWithAssert("config", "yieldtimems"))
                : 0;

            // get a defaulted message to fire on the loop context on each iteration
            string message = this.Configuration.GetOverride("loop-message", "work");

            // create a context and copy params and current control state into it
            LoopProcessContext loopContext = this.ConfigureLoop(context);

            bool firstTime = true;

            // get our enumerator from the custom condition method
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
                    if (!firstTime && yieldTimeMs > 0)
                    {
                        System.Threading.Thread.Sleep(yieldTimeMs);
                    }
                    firstTime = false;

                    loopContext.Fire(new Event(loopContext, message));
                    
                } while (enumerator.Current);
            }

            // call optional custom code at the end of all iterations
            this.Finish(ev, loopContext);
        }
    }
}