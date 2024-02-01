using Dapper;
using DbLib;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;

namespace SelDistGroupLib.Models
{
    public class DistGroupComboLoader
    {
        public static IList<DistGroup> GetDistGroupCombos(string DT_DELIVERY, bool bAll)
        {
            using (var con = DbFactory.CreateConnection())
            {
                string cdBlock = BlockLoader.GetBlock();

                var sql = "select "
                    + "CD_DIST_GROUP, "
                    + "NM_DIST_GROUP, "
                    + "CD_KYOTEN "
                    + "from TB_DIST "
                    + "inner join TB_DIST_MAPPING on TB_DIST.ID_DIST = TB_DIST_MAPPING.ID_DIST "
                    + "where DT_DELIVERY = @DT_DELIVERY "
                    + (bAll ? "" : "and CD_BLOCK = @cdBlock ")
                    + "group by CD_DIST_GROUP, NM_DIST_GROUP, CD_KYOTEN "
                    + "order by CD_DIST_GROUP";

                return con.Query(sql, new { DT_DELIVERY, cdBlock })
                    .Select(q => new DistGroup
                    {
                        CdDistGroup = q.CD_DIST_GROUP,
                        NmDistGroup = q.NM_DIST_GROUP,
                        CdKyoten = q.CD_KYOTEN
                    }).ToList();
            }
        }
    }
}
