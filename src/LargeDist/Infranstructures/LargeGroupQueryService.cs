using Dapper;
using DbLib;
using LargeDist.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LargeDist.Infranstructures
{
    internal class LargeGroupQueryService
    {
        internal static IEnumerable<LargeDistGroup> GetAll(DateTime deliveryDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = @"select ttt1.CD_LARGE_GROUP, CD_LARGE_GROUP_NAME, OrderItemCount, ResultItemCount
                    from (
                        select CD_LARGE_GROUP, count(*) OrderItemCount, count(completed) ResultItemCount
                        from
                        (select t2.CD_LARGE_GROUP, case when sum(NU_LOPS) = sum(NU_LRPS) then 1 else null end completed
                            from TB_DIST t1
                            left join TB_DIST_MAPPING t2 on t1.ID_DIST = t2.ID_DIST
                            where DT_DELIVERY = @date
                            group by t2.CD_LARGE_GROUP, t1.CD_HIMBAN) tt1
                        group by tt1.CD_LARGE_GROUP
                    ) ttt1
                    inner join TB_LARGE_GROUP ttt2 on ttt1.CD_LARGE_GROUP = ttt2.CD_LARGE_GROUP
                        and @date >= ttt2.DT_TEKIYOKAISHI and @date < ttt2.DT_TEKIYOMUKO
                    order by CD_LARGE_GROUP";

                return con.Query(sql, new { date = deliveryDate.ToString("yyyyMMdd") })
                    .Select(x => new LargeDistGroup
                    (
                        GetStatusText(x.OrderItemCount, x.ResultItemCount),
                        x.CD_LARGE_GROUP,
                        x.CD_LARGE_GROUP_NAME,
                        x.OrderItemCount,
                        x.OrderItemCount - x.ResultItemCount
                    ))
                    .ToArray();
            }
        }

        private static string GetStatusText(int orderItemCount, int resultItemCount)
        {
            if (resultItemCount == 0)
                return "";

            if (resultItemCount < orderItemCount)
            {
                return "途中";
            }

            return "完了";
        }
    }
}