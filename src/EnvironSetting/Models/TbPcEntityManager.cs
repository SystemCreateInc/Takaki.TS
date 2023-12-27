using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using System.Data;

namespace EnvironSetting.Models
{
    public class TbPcEntityManager
    {
        internal static void Regist(int idPc, string block)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var entity = GetEntity(con, idPc);

                if (entity is null)
                {
                    Insert(con, idPc, block);
                }
                else
                {
                    Update(con, entity, block);
                }
            }
        }

        internal static void Insert(IDbConnection con, int idPc, string block)
        {
            var entity = new TBPCEntity
            {
                IDPC = idPc,
                CDBLOCK = block,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

            con.Insert(entity);
        }

        internal static void Update(IDbConnection con, TBPCEntity entity, string block)
        {
            entity.CDBLOCK = block;
            entity.UpdatedAt = DateTime.Now;

            con.Update(entity);
        }

        private static TBPCEntity? GetEntity(IDbConnection con, int idPc)
        {
            return con.Get(new TBPCEntity { IDPC = idPc });
        }
    }
}
