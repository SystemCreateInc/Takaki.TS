using ProcessorLib;

namespace StowageSvr.Reporitories
{
    public class StowageRepositoryFactory : IRepositoryFactory<IStowageRepository>
    {
        public IStowageRepository Create()
        {
            return new StowageRepository();
        }
    }
}
