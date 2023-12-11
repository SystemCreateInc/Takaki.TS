using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using LogLib;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Picking.Models
{
    public class Shain : BindableBase
    {
        public string CdShain { get; set; } = string.Empty;
        public string NmShain { get; set; } = string.Empty;
        public string ComboText { get; set; } = string.Empty;
    }

    public class PersonMstComboCreater
    {
        public static List<Shain> GetComboLists()
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
                        ComboText = x.CDSHAIN + " " + x.NMSHAIN,
                    }).ToList();
            }
        }
    }
}
