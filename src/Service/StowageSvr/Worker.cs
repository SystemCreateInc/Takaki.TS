using ProcessorLib;

namespace StowageSvr
{
    public class Worker : ProcessorLib.ServiceWorker<StowageProcessor>
    {
        public Worker(Icon icon, StowageProcessor processor, IHostApplicationLifetime hostAppLifetime)
            : base("PROC_STOWAGE", icon, processor, hostAppLifetime)
        {
        }
    }
}