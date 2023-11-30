using Azure;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SetHenkosha.Models
{
    public class ShainComboLoader
    {
        public static IList<Shain> GetShainCombos()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");

            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMSHAINEntity>(s => s
                    .Where($"{nameof(TBMSHAINEntity.DTTEKIYOKAISHI):C} <= @date and @date < {nameof(TBMSHAINEntity.DTTEKIYOMUKO):C}")
                    .WithParameters(new { date })
                    .OrderBy($"{nameof(TBMSHAINEntity.CDSHAIN)}"))
                    .Select((x, index) => new Shain
                    {
                        CdShain = x.CDSHAIN,
                        NmShain = x.NMSHAIN,
                    }).ToList();
            }
        }
    }
}
