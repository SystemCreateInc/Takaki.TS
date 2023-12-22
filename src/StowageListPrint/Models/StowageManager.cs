using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using System.Data;

namespace StowageListPrint.Models
{
    public class StowageManager
    {
        public static void Update(List<long> ids, int largeBoxPs, int smallBoxPs, int blueBoxPs, int etcBoxPs)
        {
            using (var con = DbFactory.CreateConnection())
            using (var tr = con.BeginTransaction())
            {
                foreach (var id in ids)
                {
                    var entity = GetEntity(tr, id);
                    if (entity == null)
                    {
                        throw new Exception("更新対象のデータが見つかりません");
                    }

                    // 更新は実績数を更新する
                    switch ((BoxType)entity.STBOXTYPE)
                    {
                        case BoxType.EtcBox:
                            entity.NURBOXCNT = etcBoxPs;
                            break;
                        case BoxType.SmallBox:
                            entity.NURBOXCNT = smallBoxPs;
                            break;
                        case BoxType.LargeBox:
                            entity.NURBOXCNT = largeBoxPs;
                            break;
                        case BoxType.BlueBox:
                            entity.NURBOXCNT = blueBoxPs;
                            break;
                    }

                    con.Update(entity, x => x.AttachToTransaction(tr));
                }

                tr.Commit();
            }
        }

        private static TBSTOWAGEEntity? GetEntity(IDbTransaction tr, long id)
        {
            return tr.Connection!.Get(new TBSTOWAGEEntity { IDSTOWAGE = id }, x => x.AttachToTransaction(tr));
        }
    }
}
