using Dapper;
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

        internal static IEnumerable<DistGroupInfo> GetSameCourse(IEnumerable<string> courses, long idDistGroup, string startDate, string endDate)
        {
            using (var con = DbFactory.CreateConnection())
            {
                var limitCourses = courses
                    .Select((v, i) => new { v, i })
                    .GroupBy(x => x.i / 200)
                    .Select(g => g.Select(x => x.v));

                foreach (var splitCourses in limitCourses)
                {
                    var sameCourseInfos = con.Find<TBDISTGROUPEntity>(s => s
                    .Include<TBDISTGROUPSHUKKABATCHEntity>()
                    .Include<TBDISTGROUPLARGEGROUPEntity>()
                    .Include<TBDISTGROUPCOURSEEntity>()
                    .Where(@$"{nameof(TBDISTGROUPEntity.IDDISTGROUP):of TB_DIST_GROUP} <> {nameof(idDistGroup):P} and
                        {nameof(TBDISTGROUPCOURSEEntity.CDCOURSE):of TB_DIST_GROUP_COURSE} in {nameof(courses):P} and
                        {CreateTekiyoSql.GetFromRange()}")
                    .WithParameters(new { idDistGroup, courses = splitCourses, startDate, endDate }))
                        .Select(q => CreateDisgGroupInfos(q));

                    if (sameCourseInfos.Any())
                    {
                        return sameCourseInfos;
                    }
                }

                return Enumerable.Empty<DistGroupInfo>();
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

                Courses = entity.TBDISTGROUPCOURSE?.Select(x => new Course
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
