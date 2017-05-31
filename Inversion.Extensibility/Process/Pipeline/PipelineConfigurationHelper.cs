using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;

namespace Inversion.Process.Pipeline
{
    public class PipelineConfigurationHelper
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static Type GetType(string name)
        {
            Type type = Type.GetType(name);
            if (type == null)
            {
                throw new ArgumentException(String.Format("Could not resolve type '{0}'", name));
            }
            return type;
        }

        public static void Configure(IServiceContainerRegistrar registrar, IDictionary<string, string> settings, IList<string> providerTypes)
        {
            foreach (string providerType in providerTypes)
            {
                _log.DebugFormat("configuring provider {0}", providerType);

                try
                {
                    IPipelineProvider provider = Activator.CreateInstance(GetType(providerType)) as IPipelineProvider;

                    if (provider == null)
                    {
                        throw new ArgumentException(String.Format("Could not activate provider with type name {0} as instance of IPipelineProvider", providerType));
                    }

                    provider.Register(registrar, settings);
                }
                catch (Exception ex)
                {
                    _log.DebugFormat("problem: {0}", ex.ToString());
                    throw;
                }
            }
        }
    }
}