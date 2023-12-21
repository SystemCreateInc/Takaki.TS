using DbLib;
using DbLib.DbEntities;
using StowageSvr.Models;

namespace StowageSvr.Reporitories
{
    public interface IStowageRepository : IDisposable
    {
        AppLock Lock();
        void Commit();
        IEnumerable<TBSTOWAGEEntity> GetStowageEntitys(string block, string deliveryDate,
            string? distGroup = null, string? tdCode = null, string? batch = null, string? tokuisaki = null);
        IEnumerable<TBSTOWAGEEntity> GetStowageEntitys(IEnumerable<long> stowageIds);
        void UpdateStowageEntity(Stowage stowage);
    }
}
