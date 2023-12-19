using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using ImTools;
using SeatThreshold.Models;
using TakakiLib.Models;

namespace Customer.Models
{
    public class BlockEntityManager
    {
        internal static void Regist(ThresholdInfo thresholdInfo, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var entity = new TBBLOCKEntity
                {
                    CDKYOTEN = thresholdInfo.CdKyoten,
                    CDBLOCK = thresholdInfo.CdBlock,
                    STTDUNITTYPE = (int)thresholdInfo.TdUnitType,
                    NUTDUNITCNT = thresholdInfo.NuTdunitCnt,
                    NUTHRESHOLD = thresholdInfo.NuThreshold,

                    DTTEKIYOKAISHI = thresholdInfo.Tekiyokaishi,
                    DTTEKIYOMUKO = thresholdInfo.TekiyoMuko,
                    CDHENKOSHA = shainInfo.HenkoshaCode,
                    NMHENKOSHA = shainInfo.HenkoshaName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };

                con.Insert(entity);
            }
        }

        internal static void Update(ThresholdInfo thresholdInfo, ShainInfo shainInfo)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var entity = con.Get(new TBBLOCKEntity { IDBLOCK = thresholdInfo.BlockId });

                if (entity is null)
                {
                    throw new Exception("更新対象のデータが見つかりません");
                }

                entity.STTDUNITTYPE = (int)thresholdInfo.TdUnitType;
                entity.NUTDUNITCNT = thresholdInfo.NuTdunitCnt;
                entity.NUTHRESHOLD = thresholdInfo.NuThreshold;

                entity.DTTEKIYOKAISHI = thresholdInfo.Tekiyokaishi;
                entity.DTTEKIYOMUKO = thresholdInfo.TekiyoMuko;
                entity.CDHENKOSHA = shainInfo.HenkoshaCode;
                entity.NMHENKOSHA = shainInfo.HenkoshaName;
                entity.UpdatedAt = DateTime.Now;

                con.Update(entity);
            }
        }
    }
}
