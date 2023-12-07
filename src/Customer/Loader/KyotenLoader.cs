using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FastCrud;

namespace Customer.Loader
{
    public class KyotenLoader
    {
        public static string GetName(string code)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMKYOTENEntity>(s => s
                .Where($"{nameof(TBMKYOTENEntity.CDKYOTEN):C} = {nameof(code):P}")
                .WithParameters(new { code }))
                    .Select(q => q.NMKYOTEN)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
    }
}
