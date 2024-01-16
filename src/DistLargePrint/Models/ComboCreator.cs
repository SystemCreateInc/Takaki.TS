using DbLib;
using DbLib.Defs;
using Dapper;

namespace DistLargePrint.Models
{
    public class ComboCreator
    {
        public static List<Combo> Create(string deliveryDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = "select "
                    + "DT_DELIVERY, "
                    + "CD_LARGE_GROUP, "
                    + "NM_LARGE_GROUP "
                    + "from TB_DIST "
                    + "inner join TB_DIST_MAPPING on TB_DIST.ID_DIST = TB_DIST_MAPPING.ID_DIST "
                    + "where DT_DELIVERY = @deliveryDate and FG_MAPSTATUS = @mapStatus "
                    + "group by DT_DELIVERY, CD_LARGE_GROUP, NM_LARGE_GROUP "
                    + "order by CD_LARGE_GROUP";

                return con.Query(sql, new { deliveryDate, mapStatus = MapStatus.Completed })
                    .Select((value, index) => new Combo
                    {
                        CdLargeGroup = value.CD_LARGE_GROUP,
                        NmLargeGroup = value.NM_LARGE_GROUP,
                    }).ToList();
            }
        }
    }
}
