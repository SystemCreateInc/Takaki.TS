using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FastCrud;

namespace EnvironSetting.Loader
{
    public class PcEnvironLoader
    {
        public static string? GetBlock(int idPc)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBPCEntity>(s => s
                .Where($"{nameof(TBPCEntity.IDPC):C} = {nameof(idPc):P}")
                .WithParameters(new { idPc }))
                    .Select(q => new { q.CDBLOCK })
                    .FirstOrDefault()?.CDBLOCK;
            }
        }
    }
}
