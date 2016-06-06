using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Inversion.Process;
using Inversion.Process.Behaviour;
using Inversion.Process.Pipeline;
using Inversion.Extensibility.Extensions;

namespace Inversion.Extensibility
{
    public class Prototypes : IPipelineProvider
    {
        public void Register(IServiceContainerRegistrar registrar, IDictionary<string, string> settings)
        {
            Prototype.NamedCases["control-state-eval"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "eval"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "eval");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return res != null && String.Equals(res, e.Value, StringComparison.CurrentCultureIgnoreCase);
                    });
                });

            Prototype.NamedCases["control-state-eval-negative"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "eval-negative"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "eval-negative");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return res != null && !String.Equals(res, e.Value, StringComparison.CurrentCultureIgnoreCase);
                    });
                });

            Prototype.NamedCases["control-state-eval-empty"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "eval-empty"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "eval-empty");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return String.IsNullOrEmpty(res);
                    });
                });

            Prototype.NamedCases["control-state-eval-not-empty"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "eval-notempty"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "eval-notempty");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return !String.IsNullOrEmpty(res);
                    });
                });

            Prototype.NamedCases["control-state-eval-contains"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "eval-contains"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "eval-contains");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return res.Contains(e.Value);
                    });
                });

            Prototype.NamedCases["control-state-eval-not-contains"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "eval-not-contains"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "eval-not-contains");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return !res.Contains(e.Value);
                    });
                });

            Prototype.NamedCases["object-cache-includes"] = new Prototype.Case(
                match: (config) => config.Has("object-cache", "includes"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("object-cache", "includes");
                    return elements.All(e => ev.Context.ObjectCache.Contains(e.Name));
                });

            Prototype.NamedCases["object-cache-excludes"] = new Prototype.Case(
                match: (config) => config.Has("object-cache", "excludes"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("object-cache", "excludes");
                    return elements.All(e => !ev.Context.ObjectCache.Contains(e.Name));
                });

            Prototype.NamedCases["context-type"] = new Prototype.Case(
                match: (config) => config.Has("context", "type"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("context", "type");
                    return elements.All(e =>
                    {
                        if (ev.Context.HasParams(e.Name))
                        {
                            string value = ev.Context.Params[e.Name];
                            Type t = Type.GetType(e.Value);
                            TypeConverter converter = TypeDescriptor.GetConverter(t);
                            if (converter.CanConvertFrom(typeof (string)))
                            {
                                try
                                {
                                    object newValue = converter.ConvertFromInvariantString(value);
                                    if (newValue != null)
                                    {
                                        return true;
                                    }
                                }
                                catch { }
                            }
                        }
                        return false;
                    });
                });
        }
    }
}