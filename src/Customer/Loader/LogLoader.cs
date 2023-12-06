using Customer.Models;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;

namespace Customer.Loader
{
    public class LogLoader
    {
        public static IEnumerable<LogInfo> Get(string cdKyoten, string cdSumTokuisaki)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBSUMTOKUISAKIEntity>(s => s
                .Where($"{nameof(TBSUMTOKUISAKIEntity.CDKYOTEN):C} = {nameof(cdKyoten):P} and {nameof(TBSUMTOKUISAKIEntity.CDSUMTOKUISAKI):C} = {nameof(cdSumTokuisaki):P}")
                .OrderBy($"{nameof(TBSUMTOKUISAKIEntity.DTTEKIYOKAISHI)} desc")
                .WithParameters(new { cdKyoten, cdSumTokuisaki }))
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
