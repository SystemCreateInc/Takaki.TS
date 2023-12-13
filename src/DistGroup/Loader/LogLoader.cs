using Customer.Models;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;

namespace DistGroup.Loader
{
    public class LogLoader
    {
        public static IEnumerable<LogInfo> Get(string cdKyoten, string CdDistGroup)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPEntity>(s => s
                .Where($"{nameof(TBDISTGROUPEntity.CDKYOTEN):C} = {nameof(cdKyoten):P} and {nameof(TBDISTGROUPEntity.CDDISTGROUP):C} = {nameof(CdDistGroup):P}")
                .OrderBy($"{nameof(TBDISTGROUPEntity.DTTEKIYOKAISHI)} desc")
                .WithParameters(new { cdKyoten, CdDistGroup }))
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
