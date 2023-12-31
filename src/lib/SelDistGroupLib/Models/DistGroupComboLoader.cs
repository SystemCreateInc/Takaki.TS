﻿using Azure;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelDistGroupLib.Models
{
    public class DistGroupComboLoader
    {
        public static IList<DistGroup> GetDistGroupCombos(string DT_DELIVERY)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPEntity>(s => s
                    .Where($"{nameof(TBDISTGROUPEntity.DTTEKIYOKAISHI):C} <= @DT_DELIVERY and @DT_DELIVERY < {nameof(TBDISTGROUPEntity.DTTEKIYOMUKO):C}")
                    .WithParameters(new { DT_DELIVERY })
                    .OrderBy($"{nameof(TBDISTGROUPEntity.CDDISTGROUP)}"))
                    .Select((x, index) => new DistGroup
                    {
                        CdDistGroup = x.CDDISTGROUP,
                        NmDistGroup = x.NMDISTGROUP,
                        CdKyoten = x.CDKYOTEN,
                    }).ToList();
            }
        }
    }
}
