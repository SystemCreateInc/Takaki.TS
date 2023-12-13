using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using Microsoft.Extensions.Configuration;
using ReferenceLogLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReferenceLogLib
{
    public static class ShainQueryService
    {
        public static Shain? Get()
        {
            using (var con = DbFactory.CreateConnection())
            {
                var entity = con.Get(new TBPCEntity { IDPC = GetPcId() }, null);
                if (entity is null)
                {
                    return null;
                }

                return new Shain(entity.CDHENKOSHA, entity.NMHENKOSHA);
            }
        }

        private static int GetPcId()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            return int.TryParse(config.GetSection("pc")?["idpc"], out var result) ? result : 1;
        }
    }
}
