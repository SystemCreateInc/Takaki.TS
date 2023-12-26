using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakakiLib.Models;
using Dapper.FastCrud;

namespace EnvironSetting.Models
{
    public class TbPcEntityManager
    {
        internal static void Insert(int idPc, string block)
        {
            using (var con = DbFactory.CreateConnection())
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
        }

        internal static void Update(int _idPc, string block)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var entity = con.Get(new TBPCEntity { IDPC = _idPc });

                if (entity is null)
                {
                    throw new Exception("更新対象のデータが見つかりません");
                }

                entity.CDBLOCK = block;
                entity.UpdatedAt = DateTime.Now;

                con.Update(entity);
            }
        }
    }
}
