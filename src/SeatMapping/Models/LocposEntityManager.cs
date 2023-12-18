using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FastCrud;
using System.Data;
using static Dapper.SqlMapper;

namespace SeatMapping.Models
{
    internal class LocposEntityManager
    {
        internal static void Regist(string block, string addrCode, short stRemove)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var targetEntity = con.Get(new TBLOCPOSEntity { CDBLOCK = block, Tdunitaddrcode = addrCode });

                if(targetEntity is null)
                {
                    Insert(con, block, addrCode, stRemove);
                }
                else
                {
                    Update(con, targetEntity, stRemove);
                }
            }
        }

        private static void Insert(IDbConnection con, string block, string addrCode, short stRemove)
        {
            var entity = new TBLOCPOSEntity
            {
                CDBLOCK = block,
                Tdunitaddrcode = addrCode,
                STREMOVE = stRemove,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            con.Insert(entity);
        }

        private static void Update(IDbConnection con, TBLOCPOSEntity entity, short stRemove)
        {
            entity.STREMOVE = stRemove;
            entity.UpdatedAt = DateTime.Now;

            con.Update(entity);
        }

        internal static bool IsExistEntity(string block, string addrCode)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Get(new TBLOCPOSEntity { CDBLOCK = block, Tdunitaddrcode = addrCode }) is not null;
            }
        }

        internal static void DeleteNotExistAddr(IEnumerable<string> tdAddrs, string block)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var locPosAddrs = con.Find<TBLOCPOSEntity>(s => s
                .Where($"{nameof(TBLOCPOSEntity.CDBLOCK):C} = {nameof(block):P}")
                .WithParameters(new { block }))
                    .Select(q => q.Tdunitaddrcode);

                // 表示器側に無いアドレスを削除対象に
                var deleteAddrs = locPosAddrs.Except(tdAddrs);

                if (deleteAddrs.Any())
                {
                    var sql = "Delete TB_LOCPOS where CD_BLOCK = @block and tdunitaddrcode in @deleteAddrs";

                    con.Query(sql, new { block, deleteAddrs });
                }
            }
        }
    }
}
