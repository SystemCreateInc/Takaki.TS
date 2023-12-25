using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ReferenceLogLib.Models;

namespace DistGroup.Loader
{
    public class LogLoader
    {
        public static IEnumerable<LogInfo> Get(string CdDistGroup)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPEntity>(s => s
                .Where($"{nameof(TBDISTGROUPEntity.CDDISTGROUP):C} = {nameof(CdDistGroup):P}")
                .OrderBy($"{nameof(TBDISTGROUPEntity.DTTEKIYOKAISHI)} desc")
                .WithParameters(new { CdDistGroup }))
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
