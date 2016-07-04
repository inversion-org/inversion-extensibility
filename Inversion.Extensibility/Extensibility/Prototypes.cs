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
            Prototype.NamedCases["control-state-equals"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "equals"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "equals");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return res != null && String.Equals(res, e.Value, StringComparison.CurrentCultureIgnoreCase);
                    });
                });

            Prototype.NamedCases["control-state-not-equals"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "not-equals"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "not-equals");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return res != null && !String.Equals(res, e.Value, StringComparison.CurrentCultureIgnoreCase);
                    });
                });

            Prototype.NamedCases["control-state-empty"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "empty"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "empty");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return String.IsNullOrEmpty(res);
                    });
                });

            Prototype.NamedCases["control-state-not-empty"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "not-empty"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "not-empty");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return !String.IsNullOrEmpty(res);
                    });
                });

            Prototype.NamedCases["control-state-value-contains"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "value-contains"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "value-contains");
                    return elements.All(e =>
                    {
                        string res = ev.Context.ControlState.GetEffectiveStringResult(e.Name);
                        return res.Contains(e.Value);
                    });
                });

            Prototype.NamedCases["control-state-value-does-not-contain"] = new Prototype.Case(
                match: (config) => config.Has("control-state", "value-does-not-contain"),
                criteria: (config, ev) =>
                {
                    IEnumerable<IConfigurationElement> elements = config.GetElements("control-state", "value-does-not-contain");
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