using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class ParseArgumentFromParameterBehaviour : PrototypedBehaviour
    {
        public ParseArgumentFromParameterBehaviour(string respondsTo) : base(respondsTo) { }
        public ParseArgumentFromParameterBehaviour(string respondsTo, IPrototype prototype) : base(respondsTo, prototype) { }
        public ParseArgumentFromParameterBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config) : base(respondsTo, config) { }

        public override void Action(IEvent ev, IProcessContext context)
        {
            /*
                                            {"config", "output-param", "size"},
                                            {"config", "input-param", "args" },
                                            {"config", "remove", "true" },
                                            {"config", "argument-position", "3" },
                                            {"config", "separator", "/" }
             */

            string outputParam = this.Configuration.GetNameWithAssert("config", "output-param");
            string inputParam = this.Configuration.GetNameWithAssert("config", "input-param");
            int argumentPosition = Convert.ToInt32(this.Configuration.GetNameWithAssert("config", "argument-position"));
            string separator = this.Configuration.GetNameWithAssert("config", "separator");
            bool remove = Convert.ToBoolean(this.Configuration.GetNameWithAssert("config", "remove"));

            string source = context.Params.GetWithAssert(inputParam);

            string[] split = source.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            context.Params[outputParam] = split[argumentPosition];

            if (remove)
            {
                string target = String.Empty;

                for (int x = 0; x < split.GetLength(0); x++)
                {
                    if (x != argumentPosition)
                    {
                        target = String.Format("{0}{1}{2}", target, (x > 0 ? separator : String.Empty), split[x]);
                    }
                }

                context.Params[inputParam] = target;
            }
        }
    }
}
