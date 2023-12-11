using DbLib.DbEntities;
using DbLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FastCrud;

namespace ReferenceLogLib
{
    public static class KyotenQueryService
    {
        public static string GetName(string code, DateTime referenceDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMKYOTENEntity>(s => s
                    .Where($@"{nameof(TBMKYOTENEntity.CDKYOTEN):C} = {nameof(code):P} 
                        and @date >= {nameof(TBMKYOTENEntity.DTTEKIYOKAISHI):C}
                        and @date < {nameof(TBMKYOTENEntity.DTTEKIYOMUKO):C}")
                    .WithParameters(new { code, date = referenceDate.ToString("yyyyMMdd") }))
                    .Select(q => q.NMKYOTEN)
                    .FirstOrDefault() ?? string.Empty;
            }
        }
    }
}
