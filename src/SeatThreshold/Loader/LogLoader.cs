using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ReferenceLogLib.Models;

namespace SeatThreshold.Loader
{
    public class LogLoader
    {
        public static IEnumerable<LogInfo> Get(string cdKyoten, string cdBlock)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBBLOCKEntity>(s => s
                .Where($"{nameof(TBBLOCKEntity.CDKYOTEN):C} = {nameof(cdKyoten):P} and {nameof(TBBLOCKEntity.CDBLOCK):C} = {nameof(cdBlock):P}")
                .OrderBy($"{nameof(TBBLOCKEntity.DTTEKIYOKAISHI)} desc")
                .WithParameters(new { cdKyoten, cdBlock }))
                    .Select(s => new LogInfo
                    {
                        StartDate = s.DTTEKIYOKAISHI,
                        EndDate = s.DTTEKIYOMUKO,
                        ShainCode = s.CDHENKOSHA,
                    });
            }
        }
    }
}
