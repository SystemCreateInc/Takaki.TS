using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DistBlock.Models;
using ReferenceLogLib.Models;

namespace DistBlock.Loader
{
    public class BlockLoader
    {
        internal static int? GetBlockCount(string cdBlock, string selectDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBBLOCKEntity>(s => s
                        .Where(@$"{nameof(TBBLOCKEntity.CDBLOCK):C} = {nameof(cdBlock):P} and
                                {CreateTekiyoSql.GetFromDate()}")
                        .WithParameters(new { cdBlock, selectDate }))
                        .Select(q => q.NUTDUNITCNT).FirstOrDefault();
            }
        }
    }
}
