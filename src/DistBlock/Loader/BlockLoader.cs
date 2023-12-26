using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DistBlock.Models;
using TakakiLib.Models;

namespace DistBlock.Loader
{
    public class BlockLoader
    {
        internal static int? GetBlockCount(string cdBlock, string selectDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                // MEMO:結果0件時、nullではなく0が返る為匿名型に
                return con.Find<TBBLOCKEntity>(s => s
                        .Where(@$"{nameof(TBBLOCKEntity.CDBLOCK):C} = {nameof(cdBlock):P} and
                                {CreateTekiyoSql.GetFromDate()}")
                        .WithParameters(new { cdBlock, selectDate }))
                        .Select(x => new { x.NUTDUNITCNT })
                        .FirstOrDefault()?.NUTDUNITCNT;                
            }
        }
    }
}
