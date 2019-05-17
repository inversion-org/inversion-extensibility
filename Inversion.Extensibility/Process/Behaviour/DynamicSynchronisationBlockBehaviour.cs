using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Inversion.Extensibility.Extensions;
using Inversion.Process;
using Inversion.Process.Behaviour;

namespace Inversion.Extensibility.Process.Behaviour
{
    public class DynamicSynchronisationBlockBehaviour : PrototypedBehaviour
    {
        private readonly IList<IProcessBehaviour> _block;

        private static readonly object _locksAccess = new object();

        protected static readonly IDictionary<string, object> Locks = new Dictionary<string, object>();

        public DynamicSynchronisationBlockBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config,
            IList<IProcessBehaviour> block) : base(respondsTo, config)
        {
            _block = block;
        }

        public override void Action(IEvent ev, IProcessContext context)
        {
            if (this.Configuration.Has("config", "flag"))
            {
                string flagName = this.Configuration.GetNameWithAssert("config", "flag");

                // set flag to show we have dispatched
                context.Flags.Add(flagName);
            }

            string lockValue = this.Configuration.GetNameWithAssert("config", "lock-template");

            foreach (Match match in Regex.Matches(lockValue, @"\{(.*?)\}"))
            {
                string keyAndPath = match.Groups[1].Value;
                string result = keyAndPath.StartsWith("param:")
                    ? context.Params.GetParameterValue(keyAndPath)
                    : context.ControlState.GetEffectiveStringResult(keyAndPath);
                lockValue = lockValue.Replace(match.Value, result);
            }

            object syncObject = null;

            try
            {
                lock (_locksAccess)
                {
                    if (!Locks.ContainsKey(lockValue))
                    {
                        Locks.Add(lockValue, new object());
                    }

                    syncObject = Locks[lockValue];
                }

                lock (syncObject)
                {
                    foreach (IProcessBehaviour behaviour in _block.Where(behaviour => behaviour.Condition(ev, context)))
                    {
                        ProcessContext.PreAction?.Invoke(behaviour, new ActionEventArgs(context: context, ev: ev));
                        behaviour.Action(ev, context);
                        ProcessContext.PostAction?.Invoke(behaviour, new ActionEventArgs(context: context, ev: ev));
                    }
                }
            }
            finally
            {
                lock (_locksAccess)
                {
                    if (Locks.ContainsKey(lockValue))
                    {
                        Locks.Remove(lockValue);
                    }
                }
            }
        }
    }
}
