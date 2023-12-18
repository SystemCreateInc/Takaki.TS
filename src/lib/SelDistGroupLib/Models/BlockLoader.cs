using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelDistGroupLib.Models
{
    public class BlockLoader
    {
        public static string GetBlock()
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("common.json", true, true)
            .Build();

            var idpc = int.Parse(config.GetSection("pc")?["idpc"] ?? "1");

            using (var con = DbFactory.CreateConnection())
            {
                var r = con.Get(new TBPCEntity { IDPC = idpc });
                return r == null ? string.Empty : r.CDBLOCK;
            }
        }
        public static string GetKyoten()
        {
            var config = new ConfigurationBuilder()
            .AddJsonFile("common.json", true, true)
            .Build();

            return config.GetSection("pc")?["cdkyoten"] ?? "4201";
        }
    }
}
