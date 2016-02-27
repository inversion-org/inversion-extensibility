using System.Collections.Generic;

namespace Inversion.Process.Pipeline
{
    public interface IPipelineProvider
    {
        void Register(IServiceContainerRegistrar registrar, IDictionary<string, string> settings);
    }
}