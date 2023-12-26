using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakakiLib.Models;
using Dapper.FastCrud;

namespace EnvironSetting.Loader
{
    internal class BlockLoader
    {
        internal static IEnumerable<string> GetBlocks()
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBBLOCKEntity>(s => s
                        .Where($"{CreateTekiyoSql.GetFromDate()}")
                        .OrderBy($"{nameof(TBBLOCKEntity.CDBLOCK)}")
                        .WithParameters(new { selectDate = DateTime.Now.ToString("yyyyMMdd") }))
                        .Select(x => x.CDBLOCK);
            }
        }
    }
}
