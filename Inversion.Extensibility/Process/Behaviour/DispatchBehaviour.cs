using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class DispatchBehaviour : PrototypedBehaviour
    {
        /// <summary>
        /// Creates a new instance of the behaviour.
        /// </summary>
        /// <param name="respondsTo">The message the behaviour will respond to.</param>
        /// <param name="config">Configuration for the behaviour.</param>
        public DispatchBehaviour(string respondsTo, IConfiguration config) : base(respondsTo, config.Elements) { }

        /// <summary>
        /// Creates a new instance of the behaviour.
        /// </summary>
        /// <param name="respondsTo">The message the behaviour will respond to.</param>
        /// <param name="config">Configuration for the behaviour.</param>
        public DispatchBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) { }

        public override bool Condition(IEvent ev, IProcessContext context)
        {
            // don't even bother firing if our flag is already set
            return base.Condition(ev, context) && !context.IsFlagged(this.Configuration.GetNameWithAssert("config", "flag"));
        }

        /// <summary>
        /// The action to perform when the `Condition(IEvent)` is met.
        /// </summary>
        /// <param name="ev">The event to consult.</param>
        /// <param name="context">The context upon which to perform any action.</param>
        public override void Action(IEvent ev, IProcessContext context)
        {
            string flagName = this.Configuration.GetNameWithAssert("config", "flag");

            // set flag to show we have dispatched
            context.Flags.Add(flagName);

            this.FireSequenceFromConfig(context, "fire");
        }
    }
}