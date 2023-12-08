using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ReferenceLogLib.Models;
using System.Linq;

namespace TakakiLib.Models
{
    public class KyotenLoader
    {
        public static string GetName(string code, string selectDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMKYOTENEntity>(s => s
                .Where($"{nameof(TBMKYOTENEntity.CDKYOTEN):C} = {nameof(code):P} and {CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { code, selectDate }))
                    .Select(q => q.NMKYOTEN)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
    }
}
