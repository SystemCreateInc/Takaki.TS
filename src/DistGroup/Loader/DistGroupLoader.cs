using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DistGroup.Models;
using ReferenceLogLib.Models;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using static Dapper.SqlMapper;

namespace DistGroup.Loader
{
    public class DistGroupLoader
    {
        public static IEnumerable<DistGroupInfo> Get()
        {
            var sql = "SELECT"
                     + " t1.CD_KYOTEN"
                     + ",t1.CD_DIST_GROUP"
                     + ",max(t1.NM_DIST_GROUP) NM_DIST_GROUP"
                     + ",max(t1.CD_BIN_SUM) CD_BIN_SUM"
                     + ",max(t3.CD_LARGE_GROUP) CD_LARGE_GROUP"
                     + ",max(t3.CD_LARGE_GROUP_NAME) CD_LARGE_GROUP_NAME"
                     + ",max(t4.NM_KYOTEN) NM_KYOTEN"
                     + " FROM TB_DIST_GROUP t1"
                     + " left join TB_DIST_GROUP_LARGE_GROUP t2 on t2.ID_DIST_GROUP = t2.ID_DIST_GROUP"
                     + " left join TB_LARGE_GROUP t3 on t3.CD_LARGE_GROUP = t2.CD_LARGE_GROUP"
                     + " left join TB_MKYOTEN t4 on t4.CD_KYOTEN = t1.CD_KYOTEN"
                     + " group by t1.CD_KYOTEN, t1.CD_DIST_GROUP"
                     + " order by t1.CD_KYOTEN, t1.CD_DIST_GROUP";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql).Select(q => new DistGroupInfo
                {
                    CdDistGroup = q.CD_DIST_GROUP,
                    NmDistGroup = q.NM_DIST_GROUP,
                    CdKyoten = q.CD_KYOTEN,
                    NmKyoten = q.NM_KYOTEN,
                    CdBinSum = (BinSumType)q.CD_BIN_SUM,
                    CdLargeGroup = q.CD_LARGE_GROUP,
                    CdLargeGroupName = q.CD_LARGE_GROUP_NAME,
                });
            }
        }

        // 拠点、仕分グループコード、適用開始日から取得(入力DLG)
        public static DistGroupInfo? GetFromKey(string cdKyoten, string cdDistGroup, string dtTekiyoKaishi)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPEntity>(s => s
                .Include<TBDISTGROUPSHUKKABATCHEntity>()
                .Include<TBDISTGROUPLARGEGROUPEntity>()
                .Include<TBDISTGROUPCOURSEEntity>()
                .Where(@$"{nameof(TBDISTGROUPEntity.CDKYOTEN):C} = {nameof(cdKyoten):P} and
                            {nameof(TBDISTGROUPEntity.CDDISTGROUP):C} = {nameof(cdDistGroup):P} and
                            {nameof(TBDISTGROUPEntity.DTTEKIYOKAISHI):C} = {nameof(dtTekiyoKaishi):P}")
                .WithParameters(new { cdKyoten, cdDistGroup, dtTekiyoKaishi }))
                    .Select(q => CreateDisgGroupInfos(q))
                    .FirstOrDefault();
            }
        }

        internal static IEnumerable<SameCourseInfo> GetSameBatchCourse(IEnumerable<string> batchs, long idDistGroup, string startDate, string endDate)
        {
            var sql = "SELECT max(t1.ID_DIST_GROUP) ID_DIST_GROUP"
                    + ",t1.CD_SHUKKA_BATCH"
                    + ",STRING_AGG(t2.CD_COURSE, ',') Courses"
                    + ",max(t3.CD_KYOTEN) CD_KYOTEN"
                    + ",max(t3.CD_DIST_GROUP) CD_DIST_GROUP"
                    + ",max(t3.DT_TEKIYOKAISHI) DT_TEKIYOKAISHI"
                    + ",max(t3.DT_TEKIYOMUKO) DT_TEKIYOMUKO"
                    + " FROM TB_DIST_GROUP_SHUKKA_BATCH t1"
                    + " join TB_DIST_GROUP_COURSE t2 on t2.ID_DIST_GROUP = t1.ID_DIST_GROUP and t2.CD_SHUKKA_BATCH = t1.CD_SHUKKA_BATCH"
                    +$" join TB_DIST_GROUP t3 on t3.ID_DIST_GROUP = t1.ID_DIST_GROUP and t3.ID_DIST_GROUP <> @idDistGroup and {CreateTekiyoSql.GetFromRange()}"
                    + " where t1.CD_SHUKKA_BATCH in @batchs"
                    + " group by t1.CD_SHUKKA_BATCH";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { batchs, idDistGroup, startDate, endDate })
                        .Select(q => new SameCourseInfo
                        {
                            IdDistGroup = q.ID_DIST_GROUP,
                            CdKyoten = q.CD_KYOTEN,
                            CdDistGroup = q.CD_DIST_GROUP,
                            Tekiyokaishi = q.DT_TEKIYOKAISHI,
                            TekiyoMuko = q.DT_TEKIYOMUKO,

                            CdShukkaBatch = q.CD_SHUKKA_BATCH,
                            Courses = ((string)q.Courses).Split(","),
                        });
            }
        }

        private static DistGroupInfo CreateDisgGroupInfos(TBDISTGROUPEntity entity)
        {
            return new DistGroupInfo
            {
                CdKyoten = entity.CDKYOTEN,
                CdDistGroup = entity.CDDISTGROUP,
                NmDistGroup = entity.NMDISTGROUP,

                CdBinSum = (BinSumType)entity.CDBINSUM,

                Batches = entity.TBDISTGROUPSHUKKABATCH?.Select(x => new BatchInfo
                {
                    IdDistGroup = entity.IDDISTGROUP,
                    CdShukkaBatch = x.CDSHUKKABATCH,
                    Sequence = x.NUSHUKKABATCHSEQ,
                    // 入力欄側で名称表示
                    // NmShukkaBatch
                }).ToList() ?? new List<BatchInfo>(),

                LargeDists = entity.TBDISTGROUPLARGEGROUP?.Select(x => new LargeDist
                {
                    IdDistGroup = entity.IDDISTGROUP,
                    CdLargeGroup = x.CDLARGEGROUP,
                    Sequence = x.NULARGEGROUPSEQ,
                    //NmLargeGroup
                }).ToList() ?? new List<LargeDist>(),

                IdDistGroup = entity.IDDISTGROUP,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Tekiyokaishi = entity.DTTEKIYOKAISHI,
                TekiyoMuko = entity.DTTEKIYOMUKO,
            };
        }
    }
}
