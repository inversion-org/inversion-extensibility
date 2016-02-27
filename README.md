# inversion-extensibility
> New base behaviours, extension methods and helpers for Inversion.

## Base behaviours
This assembly adds several new base behaviour classes to Inversion.Process and Inversion.Web.

The **LoopBehaviour** is used to iterate over a list of behaviours with an
optional yield time between iterations. This could be used to run a private
message bus which is passed individual items from a collection, or as the
equivalent of a while() construct.

The **BlockBehaviour** allows a list of behaviours to be run without being
explicitly attached to an event.

The **PrototypedConcomitantBehaviour** exposes the Success and Failure methods, so
it encompasses the idea of forming a tree in the application logic. The Success
and Failure events can then be configured to execute individual chains of
behaviours.

The **PrototypedEvaluatingBehaviour** enables loose coupling of data items that can
be sourced from IData objects in the context control state with individual fields accessible using a JSON Path, or from the value of context parameters. These values
are made available to the behaviour as a named key which is evaluated during when
the behaviour's Action method is called.

There are **IWebContext** versions of most of these base behaviours.

## Extension methods
There are many extension methods defined in the assembly. These have been
developed mostly to cut down on boiler-plate code that deals with fetching items
from the control state, parameters and the object cache.

There are also extension methods that allow complex **NamedCases** to be added to Condition configuration.

Some **NamedCases** that are powered by these methods are included in Inversion.Extensibility/Extensions/Prototypes.cs

## Pipeline provider framework
Helper functions for working with multi-file service container definitions.
