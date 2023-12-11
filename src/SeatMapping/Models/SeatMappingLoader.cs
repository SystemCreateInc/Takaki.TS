using DbLib.DbEntities;
using DbLib;
using DbLib.Defs;
using Dapper.FastCrud;
using Dapper;
using ReferenceLogLib.Models;

namespace SeatMapping.Models
{
    public class SeatMappingLoader
    {
        internal static IEnumerable<SeatMappingInfo> Get(int unitType, string block )
        {
            var sql = "SELECT"
                      + " tdunitaddr.tdunitaddrcode "
                      + ",TB_LOCPOS.ST_REMOVE "
                      + " FROM tdunitaddr "
                      + " left join TB_LOCPOS on tdunitaddr.tdunitaddrcode = TB_LOCPOS.tdunitaddrcode and TB_LOCPOS.CD_BLOCK = @block "
                      + " where usageid = @unitType "
                      + " order by tdunitaddrcode";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { block, unitType })
                    .Select(q => new SeatMappingInfo
                    {
                        Tdunitaddrcode = q.tdunitaddrcode,
                        RemoveType = (RemoveType)(q.ST_REMOVE ?? 0),
                    });
            }

        }
    }
}
