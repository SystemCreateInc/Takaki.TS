using DbLib.Defs;
using DbLib;
using Dapper;
using System.Windows.Controls.Primitives;
using TakakiLib.Models;

namespace SeatMapping.Models
{
    public class ComboCreator
    {
        public static List<Combo> Create(string kyotenCode)
        {
            var sql = "SELECT"
                      + " CD_BLOCK "
                      + ",ST_TDUNIT_TYPE "
                      + " FROM TB_BLOCK "
                      + $" where CD_KYOTEN = @kyotenCode and {CreateTekiyoSql.GetFromDate()}"
                      + " order by CD_BLOCK";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { kyotenCode, selectDate = DateTime.Now.ToString("yyyyMMdd") })
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
