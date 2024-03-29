﻿using Dapper;
using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DbLib.Defs;
using DistGroup.Models;
using TakakiLib.Models;
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
                     + ",t1.NM_DIST_GROUP "
                     + ",t1.CD_BIN_SUM "
                     + ",t3.CD_LARGE_GROUP "
                     + ",t3.CD_LARGE_GROUP_NAME "
                     + ",t4.NM_KYOTEN NM_KYOTEN"
                     + " FROM TB_DIST_GROUP t1"
                     + " left join TB_DIST_GROUP_LARGE_GROUP t2 on t2.ID_DIST_GROUP = t1.ID_DIST_GROUP and t2.NU_LARGE_GROUP_SEQ = 1"
                     +$" left join TB_LARGE_GROUP t3 on t3.CD_LARGE_GROUP = t2.CD_LARGE_GROUP and {CreateTekiyoSql.GetFromDate("t3.")}"
                     +$" left join TB_MKYOTEN t4 on t4.CD_KYOTEN = t1.CD_KYOTEN and {CreateTekiyoSql.GetFromDate("t4.")}"
                     +$" {CreateTekiyoSql.GetFromLastUpdateJoin("TB_DIST_GROUP", "CD_KYOTEN,CD_DIST_GROUP")}"
                     + " order by t1.CD_KYOTEN, t1.CD_DIST_GROUP";

            using (var con = DbFactory.CreateConnection())
            {
                return con.Query(sql, new { selectDate = DateTime.Now.ToString("yyyyMMdd") }).Select(q => new DistGroupInfo
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

        // 仕分グループコード、適用開始日から取得(入力DLG)
        public static DistGroupInfo? GetFromKey(string cdDistGroup, string dtTekiyoKaishi)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPEntity>(s => s
                .Include<TBDISTGROUPSHUKKABATCHEntity>()
                .Include<TBDISTGROUPLARGEGROUPEntity>()
                .Include<TBDISTGROUPCOURSEEntity>()
                .Where(@$"{nameof(TBDISTGROUPEntity.CDDISTGROUP):C} = {nameof(cdDistGroup):P} and
                            {nameof(TBDISTGROUPEntity.DTTEKIYOKAISHI):C} = {nameof(dtTekiyoKaishi):P}")
                .WithParameters(new { cdDistGroup, dtTekiyoKaishi }))
                    .Select(q => CreateDisgGroupInfos(q))
                    .FirstOrDefault();
            }
        }

        // 仕分グループコードで取得(
        public static DistGroupInfo? GetFromCode(string cdDistGroup)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPEntity>(s => s
                .Include<TBDISTGROUPSHUKKABATCHEntity>()
                .Include<TBDISTGROUPLARGEGROUPEntity>()
                .Include<TBDISTGROUPCOURSEEntity>()
                .Where($"{nameof(TBDISTGROUPEntity.CDDISTGROUP):C} = {nameof(cdDistGroup):P}")
                .WithParameters(new { cdDistGroup }))
                    .Select(q => CreateDisgGroupInfos(q))
                    .FirstOrDefault();
            }
        }

        internal static IEnumerable<DistGroupInfo> GetSameBatchDists(IEnumerable<string> batchs, long idDistGroup, string startDate, string endDate)
        {
            var sameBatchCourses = new List<DistGroupInfo>();

            using (var con = DbFactory.CreateConnection())
            {
                var limitBatchs = batchs
                    .Select((v, i) => new { v, i })
                    .GroupBy(x => x.i / 200)
                    .Select(g => g.Select(x => x.v));

                foreach (var splitBatchs in limitBatchs)
                {
                    sameBatchCourses.AddRange(con.Find<TBDISTGROUPEntity>(s => s
                    .Include<TBDISTGROUPSHUKKABATCHEntity>()
                    .Include<TBDISTGROUPLARGEGROUPEntity>()
                    .Include<TBDISTGROUPCOURSEEntity>()
                    .Where(@$"{nameof(TBDISTGROUPEntity.IDDISTGROUP):of TB_DIST_GROUP} <> {nameof(idDistGroup):P} and
                        {nameof(TBDISTGROUPSHUKKABATCHEntity.CDSHUKKABATCH):of TB_DIST_GROUP_SHUKKA_BATCH} in {nameof(batchs):P} and
                        {CreateTekiyoSql.GetFromRange()}")
                    .WithParameters(new { idDistGroup, batchs = splitBatchs, startDate, endDate }))
                        .Select(q => CreateDisgGroupInfos(q)).ToList());
                }

                return sameBatchCourses;
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

                // 1件目バッチのコースを取得(全バッチ同一コース)
                Courses = entity.TBDISTGROUPCOURSE?
                .Where(x => x.CDSHUKKABATCH == entity.TBDISTGROUPSHUKKABATCH?.FirstOrDefault()?.CDSHUKKABATCH).Select(x => new Course
                {
                    CdCourse = x.CDCOURSE,
                    NuCourseSeq = x.NUCOURSESEQ,
                }) ?? Enumerable.Empty<Course>(),

                IdDistGroup = entity.IDDISTGROUP,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Tekiyokaishi = entity.DTTEKIYOKAISHI,
                TekiyoMuko = entity.DTTEKIYOMUKO,
            };
        }
    }
}
