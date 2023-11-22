using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using LogLib;
using Microsoft.Extensions.Configuration;
using System;

namespace SetHenkosha.Models
{
    public class ShainManager
    {
        public static void Update(Shain ?shain)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("common.json", true, true)
                .Build();

            var pcid = int.Parse(config.GetSection("pc")?["pcid"] ?? "1");

            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                try
                {
                    var pc = GetPcEntity(tr, pcid) ??
                        throw new Exception("対象のPCデータがありません");

                    pc.CDHENKOSHA = shain?.CD_SHAIN;
                    pc.NMSHAIN = shain?.NM_SHAIN;
                    pc.UpdatedAt = DateTime.Now;

                    con.Update(pc, s => s.AttachToTransaction(tr));

                    tr.Commit();
                }
                catch (Exception e)
                {
                    Syslog.Error($"Exception:{e.Message}");
                    tr.Rollback();
                    throw;
                }
            }
        }

        public static TBPCEntity? GetPcEntity(System.Data.IDbTransaction tr, int pcid)
        {
            return tr.Connection!.Get(new TBPCEntity { IDPC = pcid }, x => x.AttachToTransaction(tr));
        }
    }
}
