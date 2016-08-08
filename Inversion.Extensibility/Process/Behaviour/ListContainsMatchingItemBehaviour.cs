using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

using Inversion.Collections;
using Inversion.Extensibility.Extensions;

namespace Inversion.Process.Behaviour
{
    public class ListContainsMatchingItemBehaviour : PrototypedConcomitantBehaviour
    {
        public ListContainsMatchingItemBehaviour(string respondsTo) : base(respondsTo) {}
        public ListContainsMatchingItemBehaviour(string respondsTo, IPrototype prototype, IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure) : base(respondsTo, prototype, success, failure) {}
        public ListContainsMatchingItemBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config, IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure) : base(respondsTo, config, success, failure) {}

        public override void Action(IEvent ev, IProcessContext context)
        {
            string listKey = this.Configuration.GetNameWithAssert("config", "list-key");

            string path = this.Configuration.GetNameWithAssert("config", "path");
            string value = this.Configuration.GetNameWithAssert("config", "value");

            IEnumerable<object> rawEnumerable = context.ControlState[listKey] as IEnumerable<object>;
            if (rawEnumerable == null)
            {
                return;
            }

            List<object> rawList = rawEnumerable.ToList();

            foreach (object item in rawList)
            {
                IData itemData = item as IData;
                if (itemData == null)
                {
                    continue;
                }

                string testValue = null;
                JToken resultToken = itemData.Data.SelectToken(path);
                if (resultToken is JObject)
                {
                    testValue = resultToken.ToString();
                }
                else if (resultToken != null)
                {
                    testValue = resultToken.Value<string>();
                }

                if (testValue != null)
                {
                    if (testValue == value)
                    {
                        this.Success(ev, context);
                        return;
                    }
                }
            }

            this.Failure(ev, context);
        }
    }
}