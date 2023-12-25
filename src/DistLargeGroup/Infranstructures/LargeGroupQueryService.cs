using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ReferenceLogLib.Models;

namespace DistLargeGroup.Infranstructures
{
    internal static class LargeGroupQueryService
    {
        internal static LogInfo[] GetLog(string cdLargeGroup)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBLARGEGROUPEntity>(s => s
                    .Where($"{nameof(TBLARGEGROUPEntity.CDLARGEGROUP):C} = {nameof(cdLargeGroup):P}")
                    .OrderBy($"{nameof(TBLARGEGROUPEntity.DTTEKIYOKAISHI):C}")
                    .WithParameters(new { cdLargeGroup }))
                    .Select(x => new LogInfo
                    {
                        Id = x.IDLARGEGROUP,
                        StartDate = x.DTTEKIYOKAISHI,
                        EndDate = x.DTTEKIYOMUKO,
                        ShainCode = x.CDHENKOSHA,
                    })
                    .ToArray();
            }
        }

        internal static IEnumerable<Models.DistLargeGroup> FindAll()
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select t1.CD_KYOTEN, t1.CD_LARGE_GROUP, CD_LARGE_GROUP_NAME
                    from (select CD_KYOTEN, CD_LARGE_GROUP
                        from TB_LARGE_GROUP
                        group by CD_KYOTEN, CD_LARGE_GROUP) t1

                    left join(select CD_KYOTEN, CD_LARGE_GROUP, CD_LARGE_GROUP_NAME
                        from TB_LARGE_GROUP
                        where @date >= DT_TEKIYOKAISHI and @date < DT_TEKIYOMUKO
                    ) t2
                    on t1.CD_KYOTEN = t2.CD_KYOTEN and t1.CD_LARGE_GROUP = t2.CD_LARGE_GROUP
                    order by t1.CD_KYOTEN, t1.CD_LARGE_GROUP";

                return con.Query(sql, new { date = DateTime.Today })
                    .Select(x => new Models.DistLargeGroup
                    {
                        CdKyoten = x.CD_KYOTEN,
                        CdLargeGroup = x.CD_LARGE_GROUP,
                        CdLargeGroupName = x.CD_LARGE_GROUP_NAME,
                    })
                    .ToArray();
            }
        }
    }
}