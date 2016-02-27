using Inversion.Data;

namespace Inversion.Process
{
    public class LoopProcessContext : ProcessContext
    {
        public IProcessContext RootContext { get; private set; }

        public LoopProcessContext(IProcessContext rootContext, IServiceContainer services, IResourceAdapter resources) : base(services, resources)
        {
            this.RootContext = rootContext;
        }
    }
}