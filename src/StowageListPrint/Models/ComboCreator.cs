﻿using DbLib;
using Dapper;
using Dapper.FastCrud;
using DbLib.DbEntities;
using TakakiLib.Models;

namespace StowageListPrint.Models
{
    public class ComboCreator
    {
        public static List<Combo> Create(string deliveryDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var sql = "select "
                    + "CD_DIST_GROUP, "
                    + "NM_DIST_GROUP "
                    + "from TB_STOWAGE "
                    + "inner join TB_STOWAGE_MAPPING on TB_STOWAGE.ID_STOWAGE = TB_STOWAGE_MAPPING.ID_STOWAGE "
                    + "where DT_DELIVERY = @deliveryDate "
                    + "group by CD_DIST_GROUP, NM_DIST_GROUP "
                    + "order by CD_DIST_GROUP";

                return con.Query(sql, new { deliveryDate })
                    .Select((value, index) => new Combo
                    {
                        CdDistGroup = value.CD_DIST_GROUP,
                        NmDistGroup = value.NM_DIST_GROUP,
                    }).ToList();
            }
        }

        public static List<ShainCombo> GetShain(string deliveryDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBMSHAINEntity>(s => s
                .Where(@$"{CreateTekiyoSql.GetFromDate()}")
                .WithParameters(new { selectDate = deliveryDate }))
                    .Select(q => new ShainCombo
                    {
                        Id = q.CDSHAIN,
                        Name = q.NMSHAIN,
                    }).ToList();
            }
        }
    }
}
