using DbLib;

namespace StowageSvr.Reporitories
{
    public interface IStowageRepository : IDisposable
    {
        AppLock Lock();
        void Commit();
    }
}
