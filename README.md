[![Build Status](https://travis-ci.org/inversion-org/inversion-extensibility.svg)](https://travis-ci.org/inversion-org/inversion-extensibility)

# Inversion.Extensibility
New data type, base behaviours, extension methods and helpers for Inversion.

## NamedTextData
This is a spiritual descendant of Inversion.TextData which takes an extra name parameter. The name is used during ToXml/ToJson instead of the string 'text-data', thereby allowing a custom element to be produced.

```
    context.ControlState["message"] = NamedTextData("hello", "world")
```

yields in JSON:

```
"message": {
    "hello": "world"
}
```
or if set as the model-item:
```
{
    "hello": "world"
}
```

## Base behaviours
This assembly adds several new base behaviour classes to Inversion.Process and Inversion.Web.

There are **IWebContext** versions of most of these base behaviours.

### LoopBehaviour
The **LoopBehaviour** creates a new context with its own private message bus, plus copies of the parameters and the current contents of the control state. Behaviours are read from configuration and registered to the bus. The **LoopBehaviour**'s Action method loops until a condition method signals it should stop. During the loop it will fire a message ("work" is the default) onto the private context and an optional yield time occurs. When the condition method signals that the loop should stop, a virtual Finish method is called which can perform some housekeeping. The message it fires can be overridden in the configuration with the tuple { "config", "override", "loop-message", "(your message)" }

The condition method is implemented as a virtual method that returns an IEnumerable< bool >, so it can use 'yield return true' to continue the loop whilst keeping local state. Returning false will signal the loop to stop.

An example of use might be iterating over a list of objects, setting one item to be active in the private control state on each iteration. It can also be used as the
equivalent of a while() construct.

In this example below, **DemoLoopBehaviour** iterates over a list of strings that are in the control state with the key 'strings'. It has a yield time between firing of 10ms. Each iteration will select one string and set it in the loop's control state as 'string'.

The source code for the two behaviours in the loop aren't shown, but the general idea is that **CreateListOfStringsBehaviour** will make a DataList< string > object called 'results' unless it already exists, and the **RandomlyAddStringToListBehaviour** will use a random number generator to determine whether it should add the currently considered string to that list. They are configured to respond to any message fired in the loop's private context (the "*" acts as a wildcard).

* Remember, a DataList is in the Inversion.Collections namespace - it is an IData compatible generic list that can represent itself as JSON or XML if the control state is rendered.

When the control method calls Finish, the list of strings called 'results' is copied into the parent context with the key 'selected-strings'.

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
        new CreateListOfStringsBehaviour("*",
            new Configuration.Builder
            {
                {"config", "output-key", "results" },
                {"control-state", "excludes", "results" }
            }),

        new RandomlyAddStringToListBehaviour("*",
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
using Inversion.Collections;
using Inversion.Process;
using Inversion.Process.Behaviour;

namespace ExtensibilityDemo
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
            DataList<string> list = loopContext.RootContext.ControlState.GetWithAssert<DataList<string>>(listKey);

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
            DataList<string> results = loopContext.ControlState.GetWithAssert<DataList<string>>(resultsKey);

            // place into the parent context
            loopContext.RootContext.ControlState[outputKey] = results;
        }
    }
}
```

### BlockBehaviour
The **BlockBehaviour** allows a list of behaviours to be run without being
explicitly attached to an event. For the moment, this enables a developer to move a list of behaviours out of being registered on the bus directly, so that they are instead considered local to the parent behaviour (which might be registered directly).

In practice I'm actually using this to cut down on the amount of times I'm typing the message name in long pipelines, as I'm aware that these are prone to copy-paste errors or typos. The behaviours that are encapsulated by a **BlockBehaviour** need only have "* " as their message.

```
  new BlockBehaviour("demo",
     config: new Configuration.Builder
     {
         {"context", "match", "action", "demo" }
     },
     block: new List<IProcessBehaviour>
     {
         new SetParametersBehaviour("*",
             new Configuration.Builder
             {
                 {"context", "set", "value1", "abc" }
             }),

         new SetControlStateBehaviour("*",
             new Configuration.Builder
             {
                 {"control-state", "set", "string1", "def123" }
             })
     }),
```

### PrototypedConcomitantBehaviour
The **PrototypedConcomitantBehaviour** exposes Success and Failure methods. It encompasses the idea of forming a conditional branch in the application logic. The Success and Failure events can then be configured to execute individual chains of
behaviours.

I use concomitant behaviours extensively during parsing operations, so that I can attach a list of behaviours to the result of the parse. For example, I may wish to load some data and then fire a message if a parsing operation succeeds, or set a flag if the operation fails.

Here's an example configuration for FileExistsBehaviour. The source code for this is shown below.

```
new FileExistsBehaviour("demo",
    config: new Configuration.Builder
    {
        {"config", "filename-parameter", "input-filename" },
        {"context", "match", "action", "demo" }
    },
    success: new List<IProcessBehaviour>
    {
        new LoadTextFromFileBehaviour("*",
            new Configuration.Builder
            {
                {"config", "filename-parameter", "input-filename" },
                {"config", "output-key", "file-contents"}
            }),

        new ParameterisedSequenceBehaviour("*",
            new Configuration.Builder
            {
                {"fire", "process-file-contents" }
            })
    },
    failure: new List<IProcessBehaviour>
    {
        new ParameterisedSequenceBehaviour("*",
            new Configuration.Builder
            {
                {"fire", "file-missing" }
            })
    })
```

Here's what the FileExistsBehaviour looks like. The state of a file's existence is effectively mapped to the success or failure of the concomitant behaviour and the calling event and context objects are passed on.

```
using System.Collections.Generic;
using System.IO;

using Inversion.Process;
using Inversion.Process.Behaviour;
using Inversion.Extensibility.Extensions;

namespace ExtensibilityDemo
{
    public class FileExistsBehaviour : PrototypedConcomitantBehaviour
    {
        public FileExistsBehaviour(string respondsTo, IEnumerable<IConfigurationElement> config,
            IEnumerable<IProcessBehaviour> success, IEnumerable<IProcessBehaviour> failure)
            : base(respondsTo, config, success, failure) {}
    
        public override void Action(IEvent ev, IProcessContext context)
        {
            string filenameParameter = this.Configuration.GetNameWithAssert("config", "filename-parameter");
    
            string filename = context.Params.GetWithAssert(filenameParameter);
    
            if (File.Exists(filename))
            {
                this.Success(ev, context);
            }
            else
            {
                this.Failure(ev, context);
            }
        }
    }
}
```

### Evals
**Evals** enable loose coupling of data items that can be sourced from fields of IData objects in the control state using a JSON Path, or from the value of context parameters. These values are made available to the behaviour as a named key which are evaluated when the behaviour's Action method is called.

Given the following configuration:
```
#region demo
    
    new SetContextItemsBehaviour("process-request",
        config: new Configuration.Builder
        {
            {"control-state", "set", "message", "hello"}
        }),
    
    new DemoBehaviour("process-request",
        config: new Configuration.Builder
        {
            {"eval", "input-string", "control-state", "message" }
        }),

#endregion demo
```

And the following code inside a behaviour's Action method:
```
    IDictionary<string, string> data = this.Configuration.Evaluate(ev, context);
    string inputString = data.GetWithAssert("input-string");
```

The call to the extension method **Evaluate** in the behaviour will return a dictionary containing the values of any "eval" items in the behaviour configuration. There will be an item called 'input-string' which takes its value from the control-state item called 'message'. The **GetEffectiveStringResult** extension method is used to produce a string from the object. If the object was more complex, then a JSONPath could be used (e.g. 'message.content' if there was a property called 'content' in the IData object called 'message') to navigate into the JSON produced by the named object and a value retrieved for it. In this case, the value of the NamedTextData object called 'message' is evaluated to be 'hello'.

Evals can take their values from the control state, context parameters, event parameters or the object cache.

```
    new Configuration.Builder
    {
        {"eval", "test1", "control-state", "controlstateobject.pathname"},
        {"eval", "test2", "context", "context-parameter-name"},
        {"eval", "test3", "event", "event-parameter-name"},
        {"eval", "test4", "object-cache", "object-cache-key"}
    };
```

## General behaviours
These are behaviours that deal with Parameters, Control State items and Events.

### CopyControlStateBehaviour
Copy one or more named Control State items to new keys.

### CopyObjectValueToControlStateBehaviour
As shown above, take an evaluated value and add it as a NamedTextData object in the Control State.

### CopyParametersBehaviour
Copy one or more parameters to different parameter names.

### DispatchBehaviour
A descendant of **ParameterisedSequenceBehaviour** which records a flag to show that it has fired. The behaviour's Condition will not let it fire if the flag already exists. This can be used to catch situations where no action has been taken due to parameters or control state not matching, e.g. in an API that hasn't recognised the URL components it has been given.

```
new DispatchBehaviour("dispatch",
    new Configuration.Builder
    {
        {"config", "flag", "dispatched" },
        {"context", "match", "action", "health"},
        {"fire", "health"}
    }),
...    
// if nothing dispatched then give a 404
new SetContextItemsBehaviour("dispatch",
    new Configuration.Builder
    {
        {"context", "set", "response-code", "404"},
        {"context", "flagged", "dispatched", "false"} // if this is false then nothing dispatched
    }),
```

### FireEventFromControlStateBehaviour
Take a named IEvent object from the Control State and fire it using the current context.

### SetContextItemsBehaviour
This is a distant descendant of **BootstrapBehaviour**. It can be used to set or remove parameters and control state items. The tuples in the configuration can have a variety of effects:
```
new SetContextItemsBehaviour("*",
    new Configuration.Builder
    {
        {"control-state", "set", "success", "true" }, // ControlState["success"] =
                                                      //   new NamedTextData("sucess", "true")
        {"control-state", "remove", "image" },        // ControlState["image"] is removed
        {"context", "set", "model-item", "success" }, // Params["model-item"] = "success"
        {"context", "remove", "response-code" },      // Params["response-code"] is removed
        {"context", "set-eval", "message", "string1" } // Params["message"] =
                                                       //   ControlState.GetEffectiveStringResult("string1")
    }),
```

## Extension methods
There are many extension methods defined in the assembly. These have been
developed mostly to cut down on boiler-plate code that deals with fetching items
from the control state, parameters and the object cache.

There are also extension methods that allow complex **NamedCases** to be added to Condition configuration.

Some **NamedCases** that are powered by these methods are included in Inversion.Extensibility/Extensibility/Extensions/Prototypes.cs

## Pipeline provider framework
Helper functions for working with multi-file service container definitions.
