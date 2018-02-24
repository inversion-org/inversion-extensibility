using System;
using System.Collections.Generic;
using System.Text;
using Inversion.Process;

namespace Inversion.Extensibility.Extensions
{
    public static class IConfigurationEx
    {
        public static string GetOverride(this IConfiguration self, string name)
        {
            if (self.Has("config", "override", name))
            {
                return self.GetValue("config", "override", name);
            }

            return name;
        }

        public static string GetOverride(this IConfiguration self, string name, string value)
        {
            if (self.Has("config", "override", name))
            {
                return self.GetValue("config", "override", name);
            }

            return value;
        }

        public static string GetNameWithAssert(this IConfiguration self, string frame, string slot)
        {
            if (!self.Has(frame, slot))
            {
                throw new ArgumentException(String.Format("Expected '{0}','{1}' in configuration.", frame, slot));
            }
            return self.GetName(frame, slot);
        }

        public static string ToDiagnosticString(this IConfiguration self)
        {
            StringBuilder sb = new StringBuilder();
            foreach (IConfigurationElement element in self.Elements)
            {
                sb.AppendLine(String.Format("f:{0} s:{1} n:{2} v:{3}", element.Frame, element.Slot, element.Name,
                    element.Value));
            }
            return sb.ToString();
        }

        public static IDictionary<string, string> Evaluate(this IConfiguration self, IEvent ev, IProcessContext context)
        {
            IDictionary<string, string> dict = new Dictionary<string, string>();

            foreach (IConfigurationElement element in self.GetElements("eval"))
            {
                switch (element.Name)
                {
                    case "control-state":
                        dict[element.Slot] = context.ControlState.GetEffectiveStringResult(element.Value);
                        break;
                    case "context":
                        dict[element.Slot] = context.Params.GetWithAssert(element.Value);
                        break;
                    case "event":
                        dict[element.Slot] = ev.Params.GetWithAssert(element.Value);
                        break;
                    // case "object-cache":
                    //     dict[element.Slot] = context.ObjectCache.Get(element.Value).ToString();
                    //     break;
                }
            }

            return dict;
        }
    }
}