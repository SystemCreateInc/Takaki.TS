using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ReferenceLogLib.Models;

namespace DistBlock.Loader
{
    public class LogLoader
    {
        public static IEnumerable<LogInfo> Get(string cdKyoten, string cdDistBlock)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTBLOCKEntity>(s => s
                .Where($"{nameof(TBDISTBLOCKEntity.CDKYOTEN):C} = {nameof(cdKyoten):P} and {nameof(TBDISTBLOCKEntity.CDDISTGROUP):C} = {nameof(cdDistBlock):P}")
                .OrderBy($"{nameof(TBDISTBLOCKEntity.DTTEKIYOKAISHI)} desc")
                .WithParameters(new { cdKyoten, cdDistBlock }))
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
