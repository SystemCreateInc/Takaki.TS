using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ReferenceLogLib.Models;

namespace Customer.Loader
{
    public class LogLoader
    {
        public static IEnumerable<LogInfo> Get(string cdSumTokuisaki)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBSUMTOKUISAKIEntity>(s => s
                .Where($"{nameof(TBSUMTOKUISAKIEntity.CDSUMTOKUISAKI):C} = {nameof(cdSumTokuisaki):P}")
                .OrderBy($"{nameof(TBSUMTOKUISAKIEntity.DTTEKIYOKAISHI)} desc")
                .WithParameters(new { cdSumTokuisaki }))
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
