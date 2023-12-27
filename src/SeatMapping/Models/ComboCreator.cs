using DbLib.Defs;
using DbLib;
using Dapper;
using System.Windows.Controls.Primitives;
using TakakiLib.Models;

namespace SeatMapping.Models
{
    public class ComboCreator
    {
        public static List<Combo> Create()
        {
            var sql = "SELECT"
                      + " CD_BLOCK "
                      + ",max(ST_TDUNIT_TYPE) ST_TDUNIT_TYPE"
                      + " FROM TB_BLOCK "
                      + " group by CD_BLOCK"
                      + " order by CD_BLOCK";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { selectDate = DateTime.Now.ToString("yyyyMMdd") })
                    .Select((q, idx) => new Combo
                    {
                        Index = idx,
                        Id = q.CD_BLOCK,
                        UnitType = q.ST_TDUNIT_TYPE,
                    }).ToList();
            }
        }
    }
}
