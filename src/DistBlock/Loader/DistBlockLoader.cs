using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DistBlock.Models;
using ImTools;
using TakakiLib.Models;
using System.Windows.Controls.Primitives;

namespace DistBlock.Loader
{
    public class DistBlockLoader
    {
        public static IEnumerable<DistBlockInfo> Get()
        {
            var sql = "SELECT max(t1.ID_DIST_BLOCK) ID_DIST_BLOCK"
                    + ",t1.CD_KYOTEN"
                    + ",max(t2.NM_KYOTEN) NM_KYOTEN"
                    + ",t1.CD_DIST_GROUP"
                    + ",max(t3.NM_DIST_GROUP) NM_DIST_GROUP"
                    + " FROM TB_DIST_BLOCK t1"
                    + $" left join TB_MKYOTEN t2 on t2.CD_KYOTEN = t1.CD_KYOTEN and {CreateTekiyoSql.GetFromDate("t2.")}"
                    + $" left join TB_DIST_GROUP t3 on t3.CD_DIST_GROUP = t1.CD_DIST_GROUP and {CreateTekiyoSql.GetFromDate("t3.")}"
                    + " group by t1.CD_KYOTEN, t1.CD_DIST_GROUP"
                    + " order by t1.CD_KYOTEN, t1.CD_DIST_GROUP";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { selectDate = DateTime.Now.ToString("yyyyMMdd") })
                    .Select(q => new DistBlockInfo
                    {
                        CdKyoten = q.CD_KYOTEN,
                        NmKyoten = q.NM_KYOTEN,
                        CdDistGroup = q.CD_DIST_GROUP,
                        NmDistGroup = q.NM_DIST_GROUP,
                    });
            }
        }

        // 拠点、仕分グループ、適用日から取得(入力DLG)
        public static DistBlockInfo? GetFromKey(string cdDistGroup, string dtTekiyoKaishi)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTBLOCKEntity>(s => s
                .Include<TBDISTBLOCKSEQEntity>(x => x.InnerJoin())
                .Where(@$"{nameof(TBDISTBLOCKEntity.CDDISTGROUP):C} = {nameof(cdDistGroup):P} and
                            {nameof(TBDISTBLOCKEntity.DTTEKIYOKAISHI):C} = {nameof(dtTekiyoKaishi):P}")
                .WithParameters(new { cdDistGroup, dtTekiyoKaishi }))
                    .Select(q => CreateDistBlockInfo(q))
                    .FirstOrDefault();
            }
        }

        public static List<SameDistBlock> GetSameBlocks(IEnumerable<string> blocks, string startDate, string endDate, long excludeId)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTBLOCKSEQEntity>(s => s
                        .Include<TBDISTBLOCKEntity>()
                        .Where(@$"{nameof(TBDISTBLOCKSEQEntity.CDBLOCK):C} in {nameof(blocks):P} and
                            {nameof(TBDISTBLOCKSEQEntity.CDADDRFROM):C} is not null and
                            {nameof(TBDISTBLOCKSEQEntity.CDADDRTO):C} is not null and
                            {nameof(TBDISTBLOCKSEQEntity.IDDISTBLOCK):of TB_DIST_BLOCK_SEQ} <> @excludeId and
                            {CreateTekiyoSql.GetFromRange()}")
                .WithParameters(new { blocks, startDate, endDate, excludeId }))
                    .Select(q => new SameDistBlock
                    {
                        DistBlockId = q.IDDISTBLOCK,
                        CdKyoten = q.TBDISTBLOCK.CDKYOTEN,
                        CdDistGroup = q.TBDISTBLOCK.CDDISTGROUP,
                        CdBlock = q.CDBLOCK ?? string.Empty,                        
                        CdAddrFrom = q.CDADDRFROM!,
                        CdAddrTo = q.CDADDRTO!,

                        Tekiyokaishi = q.TBDISTBLOCK.DTTEKIYOKAISHI,
                        TekiyoMuko = q.TBDISTBLOCK.DTTEKIYOMUKO,
                    }).ToList();
            }
        }

        private static DistBlockInfo CreateDistBlockInfo(TBDISTBLOCKEntity entity)
        {
            return new DistBlockInfo
            {
                DistBlockId = entity.IDDISTBLOCK,
                CdKyoten = entity.CDKYOTEN,
                //NmKyoten
                CdDistGroup = entity.CDDISTGROUP,
                //NmDistGroup

                Blocks = entity.TBDISTBLOCKSEQ!.Select(x => new Block
                {
                    CdBlock = x.CDBLOCK ?? string.Empty,
                    NuBlockSeq = x.NUBLOCKSEQ,
                    CdAddrFrom = x.CDADDRFROM ?? string.Empty,
                    CdAddrTo = x.CDADDRTO ?? string.Empty,
                }).ToList(),

                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,

                Tekiyokaishi = entity.DTTEKIYOKAISHI,
                TekiyoMuko = entity.DTTEKIYOMUKO,
            };
        }
    }
}
