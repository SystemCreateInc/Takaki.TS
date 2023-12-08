using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using Microsoft.Extensions.Configuration;

namespace TakakiLib.Models
{
    public class ShainInfo
    {
        public string HenkoshaCode { get; set; } = string.Empty;
        public string HenkoshaName { get; set; } = string.Empty;
    }

    public class ShainLoader
    {
        public static ShainInfo? Get()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var idpc = int.Parse(config.GetSection("pc")?["idpc"] ?? "1");

            using (var con = DbFactory.CreateConnection())
            {
                var entity = con.Get(new TBPCEntity { IDPC = idpc }, null);

                if (entity is null)
                {
                    return null;
                }

                return new ShainInfo
                {
                    HenkoshaCode = entity.CDHENKOSHA ?? string.Empty,
                    HenkoshaName = entity.NMHENKOSHA ?? string.Empty,
                };
            }
        }
    }
}
