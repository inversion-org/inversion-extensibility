using System.Collections.Generic;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class CopyParametersBehaviour : PrototypedBehaviour
    {
        public CopyParametersBehaviour(string respondsTo) : base(respondsTo) {}
        public CopyParametersBehaviour(string respondsTo, IPrototype prototype) : base(respondsTo, prototype) {}
        public CopyParametersBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            foreach (IConfigurationElement element in this.Configuration.GetElements("config", "copy"))
            {
                context.Params[element.Value] = context.Params.GetWithAssert(element.Name);
            }
        }
    }
}