using DbLib.DbEntities;
using DbLib;
using DistGroup.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.FastCrud;

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
