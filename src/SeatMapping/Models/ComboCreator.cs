using Dapper;
using DbLib;

namespace SeatMapping.Models
{
    public class ComboCreator
    {
        public static List<Combo> Create()
        {
            int blockBorder = 10;

            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                blockBorder = new Settings(tr).GetInt("VirtualSeatBlockBorder", blockBorder);
            }

            var sql = "SELECT"
                      + " CD_BLOCK "
                      + ",max(ST_TDUNIT_TYPE) ST_TDUNIT_TYPE"
                      + " FROM TB_BLOCK "
                      + " group by CD_BLOCK"
                      + " order by CD_BLOCK";

            using (var con = DbFactory.CreateConnection())
            {
                // 仮想ブロック(指定数以上)を表示しない
                return con.Query(sql, new { selectDate = DateTime.Now.ToString("yyyyMMdd") })
                    .Select((q, idx) => new Combo
                    {
                        Index = idx,
                        Id = q.CD_BLOCK,
                        UnitType = q.ST_TDUNIT_TYPE,
                    }).Where(x => int.TryParse(x.Id, out int block) && block < blockBorder)
                    .ToList();
            }
        }
    }
}
