using Dapper.FastCrud;
using DbLib;
using DbLib.DbEntities;
using DistGroup.Models;

namespace DistGroup.Loader
{
    public class CourseLoader
    {
        public static IEnumerable<Course> Get(long idDistGroup, string cdShukkaBatch)
        {
            using (var con = DbFactory.CreateConnection())
            {
                return con.Find<TBDISTGROUPCOURSEEntity>(s => s
                .Where(@$"{nameof(TBDISTGROUPCOURSEEntity.IDDISTGROUP):C} = {nameof(idDistGroup):P} and
                            {nameof(TBDISTGROUPCOURSEEntity.CDSHUKKABATCH):C} = {nameof(cdShukkaBatch):P}")
                .WithParameters(new { idDistGroup, cdShukkaBatch }))
                    .Select(q => new Course
                    {
                        CdCourse = q.CDCOURSE,
                        NuCourseSeq = q.NUCOURSESEQ,
                    });
            }
        }
    }
}
