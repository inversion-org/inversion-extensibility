# inversion-extensibility
> New base behaviours, extension methods and helpers for Inversion.

## Base behaviours
This assembly adds several new base behaviour classes to Inversion.Process and Inversion.Web.

There are **IWebContext** versions of most of these base behaviours.

### LoopBehaviour
The **LoopBehaviour** creates a new context with its own private message bus, plus copies of the parameters and the current contents of the control state. Behaviours are read from configuration and registered to the bus. The LoopBehaviour's Action method loops until an inheritable condition method signals it should stop. During the loop it will fire a message onto the private context and an optional yield time occurs. When the condition method signals that the loop should stop, a virtual Finish method is called which can perform some housekeeping.

An example of use might be iterating over a list of objects, setting one item to be active in the private control state on each iteration. It can also be used as the
equivalent of a while() construct.

Here's an example configuration. In this example **DemoLoopBehaviour** iterates over a list of strings that are in the control state with the key 'strings'. It has a yield time between firing of 10ms. Each iteration will select one string and set it in the loop's control state as 'string'. The source code for the two behaviours in the loop aren't shown, but the general idea is that **CreateListOfStringsBehaviour** will make a list called 'results' if it doesn't exist, and the **RandomlyAddStringToListBehaviour** will use a random number generator to pick certain strings and add them to that list. When the control method calls Finish, the list of strings called 'results' is copied into the parent context with the key 'selected-strings'.

```
new DemoLoopBehaviour("demo",
    config: new Configuration.Builder
    {
        {"config", "yieldtimems", "10" },
        {"config", "list-key", "strings" },
        {"config", "iterator-key", "string" },
        {"config", "results-key", "results" },
        {"config", "output-key", "selected-strings" }
    },
    loop: new List<IProcessBehaviour>
    {
        new CreateListOfStringsBehaviour("work",
            new Configuration.Builder
            {
                {"config", "output-key", "results" },
                {"control-state", "excludes", "results" }
            }),

        new RandomlyAddStringToListBehaviour("work",
            new Configuration.Builder
            {
                {"config", "string-key", "string" },
                {"config", "list-key", "results" },
                {"control-state", "has", "results" }
            })
    }),
```

And here's the source code for **DemoLoopBehaviour**:
```
using System.Collections.Generic;
using Inversion.Extensibility.Extensions;
using Inversion.Process;
using Inversion.Process.Behaviour;

namespace DLCS.Application.Behaviour.Demo
{
    public class DemoLoopBehaviour : LoopBehaviour
    {
        public DemoLoopBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config,
            IList<IProcessBehaviour> loop) : base(respondsTo, config, loop) {}

        protected override IEnumerable<bool> LoopCondition(IEvent ev, LoopProcessContext loopContext)
        {
            string listKey = this.Configuration.GetNameWithAssert("config", "list-key");
            string iteratorKey = this.Configuration.GetNameWithAssert("config", "iterator-key");

            // fetch our source list of strings from parent context
            List<string> list = loopContext.RootContext.ControlState.GetWithAssert<List<string>>(listKey);

            // iterate over the items in the list
            foreach (string item in list)
            {
                // place each item in loop's control state using our iterator key
                loopContext.ControlState[iteratorKey] = item;

                // signal that the loop can run this time
                yield return true;
            }

            // signal that the loop should end
            yield return false;
        }

        protected override void Finish(IEvent ev, LoopProcessContext loopContext)
        {
            string outputKey = this.Configuration.GetNameWithAssert("config", "output-key");
            string resultsKey = this.Configuration.GetNameWithAssert("config", "results-key");

            // fetch our results list from the loop's control state
            List<string> results = loopContext.ControlState.GetWithAssert<List<string>>(resultsKey);

            // place into the parent context
            loopContext.RootContext.ControlState[outputKey] = results;
        }
    }
}
```

### BlockBehaviour
The **BlockBehaviour** allows a list of behaviours to be run without being
explicitly attached to an event. 

### PrototypedConcomitantBehaviour
The **PrototypedConcomitantBehaviour** exposes the Success and Failure methods, so
it encompasses the idea of forming a tree in the application logic. The Success
and Failure events can then be configured to execute individual chains of
behaviours.

### PrototypedEvaluatingBehaviour
The **PrototypedEvaluatingBehaviour** enables loose coupling of data items that can
be sourced from IData objects in the context control state with individual fields accessible using a JSON Path, or from the value of context parameters. These values
are made available to the behaviour as a named key which is evaluated during when
the behaviour's Action method is called.

## Extension methods
There are many extension methods defined in the assembly. These have been
developed mostly to cut down on boiler-plate code that deals with fetching items
from the control state, parameters and the object cache.

There are also extension methods that allow complex **NamedCases** to be added to Condition configuration.

Some **NamedCases** that are powered by these methods are included in Inversion.Extensibility/Extensions/Prototypes.cs

## Pipeline provider framework
Helper functions for working with multi-file service container definitions.
