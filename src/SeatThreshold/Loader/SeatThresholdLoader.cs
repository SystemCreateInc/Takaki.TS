using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using SeatThreshold.Models;
using TakakiLib.Models;

namespace SeatThreshold.Loader
{
    public class SeatThresholdLoader
    {
        public static IEnumerable<ThresholdInfo> Get()
        {
            var sql = "SELECT"
                      + " t1.ID_BLOCK ID_BLOCK"
                      + ",t1.CD_KYOTEN"
                      + ",t1.CD_BLOCK"
                      + ",t1.ST_TDUNIT_TYPE ST_TDUNIT_TYPE"
                      + ",t1.NU_TDUNIT_CNT NU_TDUNIT_CNT"
                      + ",t1.NU_THRESHOLD NU_THRESHOLD"
                      + ",t1.DT_TEKIYOKAISHI DT_TEKIYOKAISHI"
                      + " FROM TB_BLOCK t1"
                      +$" {CreateTekiyoSql.GetFromLastUpdateJoin("TB_BLOCK", "CD_KYOTEN,CD_BLOCK")}"
                      + " order by t1.CD_KYOTEN, t1.CD_BLOCK";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { selectDate = DateTime.Now.ToString("yyyyMMdd") }).Select(q => new ThresholdInfo
                {
                    CdKyoten = q.CD_KYOTEN,
                    CdBlock = q.CD_BLOCK,
                    TdUnitType = (TdUnitType)q.ST_TDUNIT_TYPE,
                    NuTdunitCnt = q.NU_TDUNIT_CNT,
                    NuThreshold = q.NU_THRESHOLD,
                });
            }
        }

        // ブロック、適用開始日から取得(入力DLG)
        public static ThresholdInfo? GetFromKey(string cdBlock, string dtTekiyoKaishi)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBBLOCKEntity>(s => s
                .Where(@$"{nameof(TBBLOCKEntity.CDBLOCK):C} = {nameof(cdBlock):P} and
                            {nameof(TBBLOCKEntity.DTTEKIYOKAISHI):C} = {nameof(dtTekiyoKaishi):P}")
                .WithParameters(new { cdBlock, dtTekiyoKaishi }))
                    .Select(q => CreateThresholdInfo(q))
                    .FirstOrDefault();
            }
        }
        public static ThresholdInfo? GetFromCode(string cdBlock)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBBLOCKEntity>(s => s
                .Where(@$"{nameof(TBBLOCKEntity.CDBLOCK):C} = {nameof(cdBlock):P}")
                .WithParameters(new { cdBlock }))
                    .Select(q => CreateThresholdInfo(q))
                    .FirstOrDefault();
            }
        }

        private static ThresholdInfo CreateThresholdInfo(TBBLOCKEntity entity)
        {
            return new ThresholdInfo
            {
                BlockId = entity.IDBLOCK,
                CdKyoten = entity.CDKYOTEN,
                CdBlock = entity.CDBLOCK,
                TdUnitType = (TdUnitType)entity.STTDUNITTYPE,
                NuTdunitCnt = entity.NUTDUNITCNT,
                NuThreshold = entity.NUTHRESHOLD,

                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,

                Tekiyokaishi = entity.DTTEKIYOKAISHI,
                TekiyoMuko = entity.DTTEKIYOMUKO,
            };
        }
    }
}
